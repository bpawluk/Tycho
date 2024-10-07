using Moq;
using TychoV2.Events;
using TychoV2.Events.Broker;
using TychoV2.Persistence;
using CTK = System.Threading.CancellationToken;

namespace Tycho.UnitTests.Persistence;

public sealed class OutboxProcessorTests : IDisposable
{
    private OutboxProcessor _sut;

    private Mock<IOutbox> _mockOutbox;
    private Mock<IEventProcessor> _mockEventProcessor;
    private OutboxProcessorSettings _settings;

    private List<Entry> _entries = [];
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

        _mockEventProcessor = new Mock<IEventProcessor>();
        _mockEventProcessor.Setup(ep => ep.Process(It.IsAny<string>(), It.IsAny<IEvent>(), It.IsAny<CTK>()))
                           .ReturnsAsync(true);

        _settings = new OutboxProcessorSettings
        {
            ConcurrencyLimit = 100,
            BatchSize = 2,
            InitialPollingInterval = TimeSpan.FromMilliseconds(10),
            MaxPollingInterval = TimeSpan.FromMilliseconds(20),
            PollingIntervalMultiplier = 2
        };

        _sut = new OutboxProcessor(_mockOutbox.Object, _mockEventProcessor.Object, _settings);
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
        _entries = Enumerable.Range(0, count).Select(_ => new Entry(string.Empty, default!)).ToList();
        _settings.BatchSize = batchSize;

        // Act
        _sut.StartPolling();
        await WaitForCompletion();

        // Assert
        Assert.Equal(ExpectedIterations, _iterations);
        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<Entry>(), It.IsAny<CTK>()), Times.Exactly(_entries.Count));
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
        _entries = [new Entry(string.Empty, default!), new Entry(string.Empty, default!)];
        _settings.InitialPollingInterval = TimeSpan.FromMilliseconds(initial);
        _settings.MaxPollingInterval = TimeSpan.FromMilliseconds(max);
        _settings.PollingIntervalMultiplier = multiplier;

        // Act
        _sut.StartPolling();
        await WaitForCompletion();

        // Assert
        Assert.Equal(ExpectedIterations, _iterations);
        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<Entry>(), It.IsAny<CTK>()), Times.Exactly(_entries.Count));
    }

    [Fact(Timeout = 500)]
    public async Task Process_WithBeingCloseToConcurrencyLimit_ReadsLessEntries()
    {
        // Arrange
        _entries = [new Entry(string.Empty, default!), new Entry(string.Empty, default!)];
        _settings.ConcurrencyLimit = _entries.Count + 1;
        _settings.MaxPollingInterval = _settings.InitialPollingInterval;

        var tcs = new TaskCompletionSource<bool>();
        _mockEventProcessor.Setup(ep => ep.Process(It.IsAny<string>(), It.IsAny<IEvent>(), It.IsAny<CTK>()))
                           .Returns(tcs.Task);

        // Act
        _sut.StartPolling();
        await WaitForCompletion();

        // Assert
        _mockOutbox.Verify(o => o.Read(It.Is<int>(value => value == 2), It.IsAny<CTK>()), Times.Once);
        _mockOutbox.Verify(o => o.Read(It.Is<int>(value => value == 1), It.IsAny<CTK>()), Times.AtLeastOnce);
    }

    [Fact(Timeout = 500)]
    public async Task Process_WithReachingTheConcurrencyLimit_StopsReadingEntries()
    {
        // Arrange
        _entries = [new Entry(string.Empty, default!), new Entry(string.Empty, default!)];
        _settings.ConcurrencyLimit = _entries.Count;
        _settings.MaxPollingInterval = _settings.InitialPollingInterval;

        var tcs = new TaskCompletionSource<bool>();
        _mockEventProcessor.Setup(ep => ep.Process(It.IsAny<string>(), It.IsAny<IEvent>(), It.IsAny<CTK>()))
                           .Returns(tcs.Task);

        // Act
        _sut.StartPolling();
        await WaitNumberOfIterations(1);

        // Assert
        _mockOutbox.Verify(o => o.Read(It.IsAny<int>(), It.IsAny<CTK>()), Times.Once);
    }

    [Fact(Timeout = 500)]
    public async Task Process_WithFailureProcessingEntry_CompletesAllAndMarksTheFailure()
    {
        // Arrange
        _entries = [new Entry(string.Empty, default!), new Entry("fail", default!), new Entry(string.Empty, default!)];
        _settings.MaxPollingInterval = _settings.InitialPollingInterval;
        _mockEventProcessor.Setup(ep => ep.Process(It.Is<string>(x => x == "fail"), It.IsAny<IEvent>(), It.IsAny<CTK>()))
                           .ReturnsAsync(false);

        // Act
        _sut.StartPolling();
        await WaitForCompletion();

        // Assert
        Assert.Equal(ExpectedIterations, _iterations);
        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<Entry>(), It.IsAny<CTK>()), Times.Exactly(2));
        _mockOutbox.Verify(o => o.MarkAsFailed(It.IsAny<Entry>(), It.IsAny<CTK>()), Times.Once);
    }

    [Fact(Timeout = 500)]
    public async Task Process_WithExceptionWhileProcessingEntry_LogsTheExceptionAndCompletesOtherEntries()
    {
        // Arrange
        _entries = [new Entry(string.Empty, default!), new Entry("throw", default!), new Entry(string.Empty, default!)];
        _settings.MaxPollingInterval = _settings.InitialPollingInterval;
        _mockEventProcessor.Setup(ep => ep.Process(It.Is<string>(x => x == "throw"), It.IsAny<IEvent>(), It.IsAny<CTK>()))
                           .ThrowsAsync(new InvalidOperationException());

        // Act
        _sut.StartPolling();
        await WaitForCompletion();

        // Assert
        Assert.Equal(ExpectedIterations, _iterations);
        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<Entry>(), It.IsAny<CTK>()), Times.Exactly(2));
        _mockOutbox.Verify(o => o.MarkAsFailed(It.IsAny<Entry>(), It.IsAny<CTK>()), Times.Never);
        // TODO: assert that the exception is logged
    }

    [Fact(Timeout = 500)]
    public async Task Process_WithExceptionWhileProcessingOutbox_LogsTheExceptionAndSkipsProcessing()
    {
        // Arrange
        _entries = [new Entry(string.Empty, default!), new Entry(string.Empty, default!)];
        _settings.MaxPollingInterval = _settings.InitialPollingInterval;
        _mockOutbox.Setup(o => o.Read(It.IsAny<int>(), It.IsAny<CTK>()))
                   .Callback(() => _iterations++)
                   .ThrowsAsync(new InvalidOperationException());

        // Act
        _sut.StartPolling();
        await WaitForCompletion();

        // Assert
        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<Entry>(), It.IsAny<CTK>()), Times.Never);
        _mockOutbox.Verify(o => o.MarkAsFailed(It.IsAny<Entry>(), It.IsAny<CTK>()), Times.Never);
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
