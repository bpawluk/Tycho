using Moq;
using TychoV2.Events;
using TychoV2.Events.Broker;
using TychoV2.Persistence;

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
        var processedBatches = 0;
        _mockOutbox = new Mock<IOutbox>();
        _mockOutbox.Setup(o => o.Read(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                   .Callback(() => _iterations++)
                   .ReturnsAsync((int count, CancellationToken _) => _entries.Skip(count * processedBatches++).Take(count).ToArray());

        _mockEventProcessor = new Mock<IEventProcessor>();
        _mockEventProcessor.Setup(ep => ep.Process(It.IsAny<string>(), It.IsAny<IEvent>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(true);

        _settings = new OutboxProcessorSettings
        {
            InitialPollingInterval = TimeSpan.FromMilliseconds(10),
            MaxPollingInterval = TimeSpan.FromMilliseconds(20),
            PollingIntervalMultiplier = 2,
            BatchSize = 2
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
        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<Entry>(), It.IsAny<CancellationToken>()), Times.Exactly(_entries.Count));
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
        _mockOutbox.Verify(o => o.MarkAsProcessed(It.IsAny<Entry>(), It.IsAny<CancellationToken>()), Times.Exactly(_entries.Count));
    }

    [Fact(Timeout = 500)]
    public async Task Process_WithBeingCloseToConcurrencyLimit_TBD()
    {

    }

    [Fact(Timeout = 500)]
    public async Task Process_WithReachingTheConcurrencyLimit_TBD()
    {

    }

    [Fact(Timeout = 500)]
    public async Task Process_WithFailureProcessingEntry_CompletesAllAndMarksTheFailure()
    {

    }

    [Fact(Timeout = 500)]
    public async Task Process_WithExceptionWhileProcessingOutbox_TBD()
    {

    }

    [Fact(Timeout = 500)]
    public async Task Process_WithExceptionWhileProcessingEntry_TBD()
    {

    }

    private async Task WaitForCompletion()
    {
        while (_iterations < ExpectedIterations)
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
