using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using Tycho.Events.Routing;
using Tycho.Persistence;
using Tycho.Persistence.Processing;
using Tycho.UnitTests._Data.Events;
using CTK = System.Threading.CancellationToken;

namespace Tycho.UnitTests.Persistence;

public sealed class OutboxProcessorTests : IDisposable
{
    private readonly Mock<IOutboxConsumer> _mockOutbox;
    private readonly OutboxActivity _outboxActivity;
    private readonly Mock<IEntryProcessor> _mockEntryProcessor;
    private readonly OutboxProcessorSettings _settings;
    private readonly FakeLogger<OutboxProcessor> _fakeLogger;


    private List<OutboxEntry> _entries = [];
    private int _iterations;

    private readonly OutboxProcessor _sut;

    public OutboxProcessorTests()
    {
        var processedEntries = 0;

        _mockOutbox = new Mock<IOutboxConsumer>();
        _mockOutbox.Setup(o => o.Read(It.IsAny<int>(), It.IsAny<CTK>()))
                   .Callback(() => _iterations++)
                   .ReturnsAsync((int count, CTK _) =>
                   {
                       var nextEntries = _entries.Skip(processedEntries).Take(count).ToArray();
                       processedEntries += nextEntries.Length;
                       return nextEntries;
                   });

        _outboxActivity = new OutboxActivity();

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

        _fakeLogger = new FakeLogger<OutboxProcessor>();

        _sut = new OutboxProcessor(
            _mockOutbox.Object, 
            _outboxActivity, 
            _mockEntryProcessor.Object, 
            _settings,
            _fakeLogger);
    }

    private int ExpectedIterations => ExpectedProcessingIterations + ExpectedIdleIterations;

    private int ExpectedProcessingIterations => (int)Math.Ceiling((double)_entries.Count / _settings.BatchSize);

    private int ExpectedIdleIterations => 1 + (int)Math.Floor(Math.Log(
        _settings.MaxPollingInterval / _settings.InitialPollingInterval,
        _settings.PollingIntervalMultiplier));

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
        _entries = Enumerable.Range(0, count)
                             .Select(_ => new OutboxEntry(
                                new HandlerIdentity(string.Empty, string.Empty, string.Empty), 
                                new TestEvent()))
                             .ToList();
        _settings.BatchSize = batchSize;

        // Act
        _sut.Initialize();
        _outboxActivity.NotifyNewEntriesAdded();
        await WaitForCompletion();

        // Assert
        Assert.True(_iterations >= ExpectedIterations);
        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<OutboxEntry>(), It.IsAny<CTK>()),
            Times.Exactly(_entries.Count));
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
            new OutboxEntry(new HandlerIdentity(string.Empty, string.Empty, string.Empty), new TestEvent()),
            new OutboxEntry(new HandlerIdentity(string.Empty, string.Empty, string.Empty), new TestEvent())
        ];
        _settings.InitialPollingInterval = TimeSpan.FromMilliseconds(initial);
        _settings.MaxPollingInterval = TimeSpan.FromMilliseconds(max);
        _settings.PollingIntervalMultiplier = multiplier;

        // Act
        _sut.Initialize();
        _outboxActivity.NotifyNewEntriesAdded();
        await WaitForCompletion();

        // Assert
        Assert.True(_iterations >= ExpectedIterations);
        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<OutboxEntry>(), It.IsAny<CTK>()),
            Times.Exactly(_entries.Count));
    }

    [Fact(Timeout = 500)]
    public async Task Process_WithBeingCloseToConcurrencyLimit_ReadsLessEntries()
    {
        // Arrange
        _entries =
        [
            new OutboxEntry(new HandlerIdentity(string.Empty, string.Empty, string.Empty), new TestEvent()),
            new OutboxEntry(new HandlerIdentity(string.Empty, string.Empty, string.Empty), new TestEvent())
        ];
        _settings.ConcurrencyLimit = _entries.Count + 1;
        _settings.MaxPollingInterval = _settings.InitialPollingInterval;

        var tcs = new TaskCompletionSource<bool>();
        _mockEntryProcessor.Setup(ep => ep.Process(It.IsAny<OutboxEntry>()))
                           .Returns(tcs.Task);

        // Act
        _sut.Initialize();
        _outboxActivity.NotifyNewEntriesAdded();
        await WaitForCompletion();

        // Assert
        Assert.True(_iterations >= ExpectedIterations);
        _mockOutbox.Verify(o => o.Read(It.Is<int>(value => value == 2), It.IsAny<CTK>()), Times.Once);
        _mockOutbox.Verify(o => o.Read(It.Is<int>(value => value == 1), It.IsAny<CTK>()), Times.AtLeastOnce);
    }

    [Fact(Timeout = 500)]
    public async Task Process_WithReachingTheConcurrencyLimit_StopsReadingEntries()
    {
        // Arrange
        _entries =
        [
            new OutboxEntry(new HandlerIdentity(string.Empty, string.Empty, string.Empty), new TestEvent()),
            new OutboxEntry(new HandlerIdentity(string.Empty, string.Empty, string.Empty), new TestEvent())
        ];
        _settings.ConcurrencyLimit = _entries.Count;
        _settings.MaxPollingInterval = _settings.InitialPollingInterval;

        var tcs = new TaskCompletionSource<bool>();
        _mockEntryProcessor.Setup(ep => ep.Process(It.IsAny<OutboxEntry>()))
                           .Returns(tcs.Task);

        // Act
        _sut.Initialize();
        _outboxActivity.NotifyNewEntriesAdded();
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
            new OutboxEntry(new HandlerIdentity(string.Empty, string.Empty, string.Empty), new TestEvent()),
            new OutboxEntry(new HandlerIdentity("fail", string.Empty, string.Empty), new TestEvent()),
            new OutboxEntry(new HandlerIdentity(string.Empty, string.Empty, string.Empty), new TestEvent())
        ];
        _mockEntryProcessor.Setup(ep => ep.Process(It.Is<OutboxEntry>(x => x.HandlerIdentity.EventId == "fail")))
                           .ReturnsAsync(false);
        _settings.MaxPollingInterval = _settings.InitialPollingInterval;

        // Act
        _sut.Initialize();
        _outboxActivity.NotifyNewEntriesAdded();
        await WaitForCompletion();

        // Assert
        Assert.True(_iterations >= ExpectedIterations);
        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<OutboxEntry>(), It.IsAny<CTK>()), Times.Exactly(2));
        _mockOutbox.Verify(o => o.MarkAsFailed(It.IsAny<OutboxEntry>(), It.IsAny<CTK>()), Times.Once);
    }

    [Fact(Timeout = 500)]
    public async Task Process_WithExceptionWhileProcessingEntry_LogsTheExceptionAndCompletesOtherEntries()
    {
        // Arrange
        var failingEntry = new OutboxEntry(new HandlerIdentity("throw", string.Empty, string.Empty), new TestEvent());
        _entries =
        [
            new OutboxEntry(new HandlerIdentity(string.Empty, string.Empty, string.Empty), new TestEvent()),
            failingEntry,
            new OutboxEntry(new HandlerIdentity(string.Empty, string.Empty, string.Empty), new TestEvent())
        ];

        var expectedException = new InvalidOperationException("Error message");
        _mockEntryProcessor.Setup(ep => ep.Process(It.Is<OutboxEntry>(x => x.HandlerIdentity.EventId == "throw")))
                           .ThrowsAsync(expectedException);

        _settings.MaxPollingInterval = _settings.InitialPollingInterval;

        // Act
        _sut.Initialize();
        _outboxActivity.NotifyNewEntriesAdded();
        await WaitForCompletion();

        // Assert
        Assert.True(_iterations >= ExpectedIterations);

        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<OutboxEntry>(), It.IsAny<CTK>()), Times.Exactly(2));
        _mockOutbox.Verify(o => o.MarkAsFailed(It.IsAny<OutboxEntry>(), It.IsAny<CTK>()), Times.Never);

        var logs = _fakeLogger.Collector.GetSnapshot();
        Assert.Single(logs);

        var log = logs.Single();
        Assert.Equal(LogLevel.Error, log.Level);
        Assert.Equal($"Entry processing failed ({failingEntry.Id})", log.Message);
        Assert.NotNull(log.Exception);
        Assert.Equal(expectedException.Message, log.Exception.Message);
    }

    [Fact(Timeout = 500)]
    public async Task Process_WithExceptionWhileProcessingOutbox_LogsTheExceptionAndSkipsProcessing()
    {
        // Arrange
        _entries =
        [
            new OutboxEntry(new HandlerIdentity(string.Empty, string.Empty, string.Empty), new TestEvent()),
            new OutboxEntry(new HandlerIdentity(string.Empty, string.Empty, string.Empty), new TestEvent())
        ];

        var expectedException = new InvalidOperationException("Error message");
        _mockOutbox.Setup(o => o.Read(It.IsAny<int>(), It.IsAny<CTK>()))
                   .Callback(() => _iterations++)
                   .ThrowsAsync(expectedException);

        _settings.MaxPollingInterval = _settings.InitialPollingInterval;

        // Act
        _sut.Initialize();
        _outboxActivity.NotifyNewEntriesAdded();
        await WaitForCompletion();

        // Assert
        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<OutboxEntry>(), It.IsAny<CTK>()), Times.Never);
        _mockOutbox.Verify(o => o.MarkAsFailed(It.IsAny<OutboxEntry>(), It.IsAny<CTK>()), Times.Never);

        var logs = _fakeLogger.Collector.GetSnapshot();
        Assert.NotEmpty(logs);

        foreach (var log in logs)
        {
            Assert.Equal(LogLevel.Error, log.Level);
            Assert.Equal("Outbox processing failed", log.Message);
            Assert.NotNull(log.Exception);
            Assert.Equal(expectedException.Message, log.Exception.Message);
        }
    }

    private async Task WaitForCompletion()
    {
        await WaitNumberOfIterations(ExpectedIterations);
    }

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