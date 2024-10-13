using Moq;
using Tycho.UnitTests._Data.Events;
using TychoV2.Events;
using TychoV2.Events.Routing;
using TychoV2.Persistence;
using TychoV2.Persistence.Processing;
using CTK = System.Threading.CancellationToken;

namespace Tycho.UnitTests.Persistence;

public sealed class OutboxProcessorTests : IDisposable
{
    private OutboxProcessor _sut;

    private Mock<IOutbox> _mockOutbox;
    private Mock<IEntryProcessor> _mockEntryProcessor;
    private OutboxProcessorSettings _settings;

    private List<OutboxEntry> _entries = [];
    private int _iterations = 0;

    private int ExpectedIterations => ExpectedProcessingIterations + ExpectedIdleIterations;

    private int ExpectedProcessingIterations => (int)Math.Ceiling((double)_entries.Count / _settings.BatchSize);

    private int ExpectedIdleIterations => 1 + (int)Math.Floor(Math.Log(
        _settings.MaxPollingInterval / _settings.InitialPollingInterval,
        _settings.PollingIntervalMultiplier));

    public OutboxProcessorTests()
    {
        var processedEntries = 0;
        _mockOutbox = new Mock<IOutbox>();
        _mockOutbox.Setup(o => o.Read(It.IsAny<int>(), It.IsAny<CTK>()))
                   .Callback(() => _iterations++)
                   .ReturnsAsync((int count, CTK _) =>
                   {
                       var nextEntries = _entries.Skip(processedEntries).Take(count).ToArray();
                       processedEntries += nextEntries.Length;
                       return nextEntries;
                   });

        _mockEntryProcessor = new Mock<IEntryProcessor>();
        _mockEntryProcessor.Setup(ep => ep.Process(It.IsAny<OutboxEntry>()))
                           .ReturnsAsync(true);

        _settings = new OutboxProcessorSettings
        {
            ConcurrencyLimit = 100,
            BatchSize = 2,
            InitialPollingInterval = TimeSpan.FromMilliseconds(10),
            MaxPollingInterval = TimeSpan.FromMilliseconds(20),
            PollingIntervalMultiplier = 2
        };

        _sut = new OutboxProcessor(_mockOutbox.Object, _mockEntryProcessor.Object, _settings);
    }

    [Theory(Timeout = 500)]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(5, 5)]
    [InlineData(10, 5)]
    [InlineData(25, 5)]
    [InlineData(100, 5)]
    public async Task Process_WithDifferentOutboxContents_CompletesAll(int count, int batchSize)
    {
        // Arrange
        _entries = Enumerable.Range(0, count).Select(_ => new OutboxEntry(new(string.Empty, string.Empty, string.Empty), new TestEvent())).ToList();
        _settings.BatchSize = batchSize;

        // Act
        _sut.Initialize();
        _mockOutbox.Raise(o => o.NewEntriesAdded += null, EventArgs.Empty);
        await WaitForCompletion();

        // Assert
        Assert.Equal(ExpectedIterations, _iterations);
        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<OutboxEntry>(), It.IsAny<CTK>()), Times.Exactly(_entries.Count));
    }

    [Theory(Timeout = 500)]
    [InlineData(10, 20, 1.5)]
    [InlineData(20, 20, 2.0)]
    [InlineData(15, 20, 2.0)]
    [InlineData(10, 20, 2.0)]
    [InlineData(10, 80, 2.0)]
    [InlineData(10, 100, 2.5)]
    public async Task Process_WithDifferentPollingIntervals_CompletesAll(int initial, int max, double multiplier)
    {
        // Arrange
        _entries =
        [
            new OutboxEntry(new(string.Empty, string.Empty, string.Empty), new TestEvent()),
            new OutboxEntry(new(string.Empty, string.Empty, string.Empty), new TestEvent())
        ];
        _settings.InitialPollingInterval = TimeSpan.FromMilliseconds(initial);
        _settings.MaxPollingInterval = TimeSpan.FromMilliseconds(max);
        _settings.PollingIntervalMultiplier = multiplier;

        // Act
        _sut.Initialize();
        _mockOutbox.Raise(o => o.NewEntriesAdded += null, EventArgs.Empty);
        await WaitForCompletion();

        // Assert
        Assert.Equal(ExpectedIterations, _iterations);
        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<OutboxEntry>(), It.IsAny<CTK>()), Times.Exactly(_entries.Count));
    }

    [Fact(Timeout = 500)]
    public async Task Process_WithBeingCloseToConcurrencyLimit_ReadsLessEntries()
    {
        // Arrange
        _entries =
        [
            new OutboxEntry(new(string.Empty, string.Empty, string.Empty), new TestEvent()),
            new OutboxEntry(new(string.Empty, string.Empty, string.Empty), new TestEvent())
        ];
        _settings.ConcurrencyLimit = _entries.Count + 1;
        _settings.MaxPollingInterval = _settings.InitialPollingInterval;

        var tcs = new TaskCompletionSource<bool>();
        _mockEntryProcessor.Setup(ep => ep.Process(It.IsAny<OutboxEntry>()))
                           .Returns(tcs.Task);

        // Act
        _sut.Initialize();
        _mockOutbox.Raise(o => o.NewEntriesAdded += null, EventArgs.Empty);
        await WaitForCompletion();

        // Assert
        _mockOutbox.Verify(o => o.Read(It.Is<int>(value => value == 2), It.IsAny<CTK>()), Times.Once);
        _mockOutbox.Verify(o => o.Read(It.Is<int>(value => value == 1), It.IsAny<CTK>()), Times.AtLeastOnce);
    }

    [Fact(Timeout = 500)]
    public async Task Process_WithReachingTheConcurrencyLimit_StopsReadingEntries()
    {
        // Arrange
        _entries =
        [
            new OutboxEntry(new(string.Empty, string.Empty, string.Empty), new TestEvent()),
            new OutboxEntry(new(string.Empty, string.Empty, string.Empty), new TestEvent())
        ];
        _settings.ConcurrencyLimit = _entries.Count;
        _settings.MaxPollingInterval = _settings.InitialPollingInterval;

        var tcs = new TaskCompletionSource<bool>();
        _mockEntryProcessor.Setup(ep => ep.Process(It.IsAny<OutboxEntry>()))
                           .Returns(tcs.Task);

        // Act
        _sut.Initialize();
        _mockOutbox.Raise(o => o.NewEntriesAdded += null, EventArgs.Empty);
        await WaitNumberOfIterations(1);

        // Assert
        _mockOutbox.Verify(o => o.Read(It.IsAny<int>(), It.IsAny<CTK>()), Times.Once);
    }

    [Fact(Timeout = 500)]
    public async Task Process_WithFailureProcessingEntry_CompletesAllAndMarksTheFailure()
    {
        // Arrange
        _entries =
        [
            new OutboxEntry(new(string.Empty, string.Empty, string.Empty), new TestEvent()),
            new OutboxEntry(new("fail", string.Empty, string.Empty), new TestEvent()),
            new OutboxEntry(new(string.Empty, string.Empty, string.Empty), new TestEvent())
        ];
        _mockEntryProcessor.Setup(ep => ep.Process(It.Is<OutboxEntry>(x => x.HandlerIdentity.EventId == "fail")))
                           .ReturnsAsync(false);
        _settings.MaxPollingInterval = _settings.InitialPollingInterval;

        // Act
        _sut.Initialize();
        _mockOutbox.Raise(o => o.NewEntriesAdded += null, EventArgs.Empty);
        await WaitForCompletion();

        // Assert
        Assert.Equal(ExpectedIterations, _iterations);
        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<OutboxEntry>(), It.IsAny<CTK>()), Times.Exactly(2));
        _mockOutbox.Verify(o => o.MarkAsFailed(It.IsAny<OutboxEntry>(), It.IsAny<CTK>()), Times.Once);
    }

    [Fact(Timeout = 500)]
    public async Task Process_WithExceptionWhileProcessingEntry_LogsTheExceptionAndCompletesOtherEntries()
    {
        // Arrange
        _entries =
        [
            new OutboxEntry(new(string.Empty, string.Empty, string.Empty), new TestEvent()),
            new OutboxEntry(new("throw", string.Empty, string.Empty), new TestEvent()),
            new OutboxEntry(new(string.Empty, string.Empty, string.Empty), new TestEvent())
        ];
        _mockEntryProcessor.Setup(ep => ep.Process(It.Is<OutboxEntry>(x => x.HandlerIdentity.EventId == "throw")))
                           .ThrowsAsync(new InvalidOperationException());
        _settings.MaxPollingInterval = _settings.InitialPollingInterval;

        // Act
        _sut.Initialize();
        _mockOutbox.Raise(o => o.NewEntriesAdded += null, EventArgs.Empty);
        await WaitForCompletion();

        // Assert
        Assert.Equal(ExpectedIterations, _iterations);
        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<OutboxEntry>(), It.IsAny<CTK>()), Times.Exactly(2));
        _mockOutbox.Verify(o => o.MarkAsFailed(It.IsAny<OutboxEntry>(), It.IsAny<CTK>()), Times.Never);
        // TODO: assert that the exception is logged
    }

    [Fact(Timeout = 500)]
    public async Task Process_WithExceptionWhileProcessingOutbox_LogsTheExceptionAndSkipsProcessing()
    {
        // Arrange
        _entries =
        [
            new OutboxEntry(new(string.Empty, string.Empty, string.Empty), new TestEvent()),
            new OutboxEntry(new(string.Empty, string.Empty, string.Empty), new TestEvent())
        ];
        _settings.MaxPollingInterval = _settings.InitialPollingInterval;
        _mockOutbox.Setup(o => o.Read(It.IsAny<int>(), It.IsAny<CTK>()))
                   .Callback(() => _iterations++)
                   .ThrowsAsync(new InvalidOperationException());

        // Act
        _sut.Initialize();
        _mockOutbox.Raise(o => o.NewEntriesAdded += null, EventArgs.Empty);
        await WaitForCompletion();

        // Assert
        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<OutboxEntry>(), It.IsAny<CTK>()), Times.Never);
        _mockOutbox.Verify(o => o.MarkAsFailed(It.IsAny<OutboxEntry>(), It.IsAny<CTK>()), Times.Never);
        // TODO: assert that the exception is logged
    }

    private async Task WaitForCompletion() => await WaitNumberOfIterations(ExpectedIterations);

    private async Task WaitNumberOfIterations(int iterationsToWait)
    {
        while (_iterations < iterationsToWait)
        {
            await Task.Delay(_settings.InitialPollingInterval);
        }
        await Task.Delay(_settings.MaxPollingInterval);
    }

    public void Dispose()
    {
        _sut.Dispose();
    }
}
