using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.Retention;
using Tycho.Persistence.EFCore.UnitTests._Utils;

namespace Tycho.Persistence.EFCore.UnitTests.Retention;

public sealed class PersistenceRetentionServiceTests : IAsyncLifetime
{
    private readonly TestTimeProvider _clock = new();
    private readonly PersistenceRetentionOptions _options = new();
    private readonly Mock<IInboxCleaner> _inbox = new();
    private readonly Mock<IOutboxCleaner> _outbox = new();
    private readonly Mock<IServiceScopeFactory> _scopeFactory = new();
    private readonly Mock<ILogger<PersistenceRetentionService>> _logger = new();
    private readonly List<Mock<IServiceScope>> _scopes = [];
    private PersistenceRetentionService _sut = default!;

    public ValueTask InitializeAsync()
    {
        _inbox.Setup(cleaner => cleaner.CleanEntriesAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _inbox.Setup(cleaner => cleaner.CleanPayloadsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _outbox.Setup(cleaner => cleaner.CleanEntriesAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _outbox.Setup(cleaner => cleaner.CleanPayloadsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _scopeFactory.Setup(factory => factory.CreateScope()).Returns(CreateScope);
        _logger.Setup(item => item.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _sut = new PersistenceRetentionService(_scopeFactory.Object, _options, _clock, _logger.Object);
        return ValueTask.CompletedTask;
    }

    [Fact]
    public async Task ExecuteAsync_WithEnabledRetention_CleansMessagesUsingCorrectCutoffs()
    {
        // Arrange
        _options.Inbox.PayloadRetention = TimeSpan.FromDays(1);
        _options.Inbox.FullCleanupRetention = TimeSpan.FromDays(2);
        _options.Outbox.PayloadRetention = TimeSpan.FromDays(3);
        _options.Outbox.FullCleanupRetention = TimeSpan.FromDays(4);

        // Act
        await RunFirstCleanupAsync();

        // Assert
        DateTime now = _clock.GetUtcNow().UtcDateTime;
        _inbox.Verify(cleaner => cleaner.CleanPayloadsAsync(now.AddDays(-1), It.IsAny<CancellationToken>()), Times.Once);
        _inbox.Verify(cleaner => cleaner.CleanEntriesAsync(now.AddDays(-2), It.IsAny<CancellationToken>()), Times.Once);
        _outbox.Verify(cleaner => cleaner.CleanPayloadsAsync(now.AddDays(-3), It.IsAny<CancellationToken>()), Times.Once);
        _outbox.Verify(cleaner => cleaner.CleanEntriesAsync(now.AddDays(-4), It.IsAny<CancellationToken>()), Times.Once);
        _inbox.VerifyNoOtherCalls();
        _outbox.VerifyNoOtherCalls();
        Assert.Single(_scopes).As<IAsyncDisposable>().Verify(scope => scope.DisposeAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCleanupAffectsEntries_LogsEachNonemptyOperation()
    {
        // Arrange
        _options.Inbox.FullCleanupRetention = TimeSpan.FromDays(2);
        _options.Inbox.PayloadRetention = TimeSpan.FromDays(1);
        _options.Outbox.FullCleanupRetention = TimeSpan.FromDays(2);
        _options.Outbox.PayloadRetention = TimeSpan.FromDays(1);

        _inbox.Setup(cleaner => cleaner.CleanEntriesAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _inbox.Setup(cleaner => cleaner.CleanPayloadsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(2);
        _outbox.Setup(cleaner => cleaner.CleanEntriesAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(3);
        _outbox.Setup(cleaner => cleaner.CleanPayloadsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(4);

        // Act
        await RunFirstCleanupAsync();

        // Assert
        DateTime cutoff = _clock.GetUtcNow().UtcDateTime.AddDays(-1);
        VerifyCleanupLog("Inbox", "Deleting messages", 1, cutoff.AddDays(-1));
        VerifyCleanupLog("Inbox", "Clearing payloads", 2, cutoff);
        VerifyCleanupLog("Outbox", "Deleting messages", 3, cutoff.AddDays(-1));
        VerifyCleanupLog("Outbox", "Clearing payloads", 4, cutoff);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCleanupAffectsNoEntries_DoesNotLogCompletion()
    {
        // Act
        await RunFirstCleanupAsync();

        // Assert
#pragma warning disable CA1873
        _logger.Verify(item => item.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
#pragma warning restore CA1873
    }

    [Theory]
    [InlineData(true, false, false, true)]
    [InlineData(false, true, true, false)]
    [InlineData(false, false, false, false)]
    public async Task ExecuteAsync_WithDisabledRetention_SkipsDisabledOperations(
        bool inboxPayloads,
        bool inboxEntries,
        bool outboxPayloads,
        bool outboxEntries)
    {
        // Arrange
        _options.Inbox.PayloadRetention = inboxPayloads ? TimeSpan.FromDays(1) : null;
        _options.Inbox.FullCleanupRetention = inboxEntries ? TimeSpan.FromDays(2) : null;
        _options.Outbox.PayloadRetention = outboxPayloads ? TimeSpan.FromDays(1) : null;
        _options.Outbox.FullCleanupRetention = outboxEntries ? TimeSpan.FromDays(2) : null;

        // Act
        await RunFirstCleanupAsync();

        // Assert
        _inbox.Verify(cleaner => cleaner.CleanPayloadsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Exactly(inboxPayloads ? 1 : 0));
        _inbox.Verify(cleaner => cleaner.CleanEntriesAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Exactly(inboxEntries ? 1 : 0));
        _outbox.Verify(cleaner => cleaner.CleanPayloadsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Exactly(outboxPayloads ? 1 : 0));
        _outbox.Verify(cleaner => cleaner.CleanEntriesAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Exactly(outboxEntries ? 1 : 0));
    }

    [Fact]
    public async Task ExecuteAsync_WithRetentionExceedingCurrentDate_ClampsCutoffsToMinimumDate()
    {
        // Arrange
        _options.Inbox.PayloadRetention = TimeSpan.MaxValue;
        _options.Outbox.FullCleanupRetention = TimeSpan.MaxValue;

        // Act
        await RunFirstCleanupAsync();

        // Assert
        _inbox.Verify(cleaner => cleaner.CleanPayloadsAsync(DateTime.MinValue, It.IsAny<CancellationToken>()), Times.Once);
        _outbox.Verify(cleaner => cleaner.CleanEntriesAsync(DateTime.MinValue, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WaitsBeforeStartingAndBetweenCleanupCycles()
    {
        // Arrange
        _options.CleanupInterval = TimeSpan.FromMinutes(15);

        // Act
        await _sut.StartAsync(TestContext.Current.CancellationToken);
        TimeSpan initialDelay = await _clock.WaitForDelayAsync();

        // Assert
        Assert.Equal(_options.InitialDelay, initialDelay);
        _scopeFactory.Verify(factory => factory.CreateScope(), Times.Never);
        _inbox.VerifyNoOtherCalls();
        _outbox.VerifyNoOtherCalls();

        // Act
        _clock.Advance(initialDelay);
        TimeSpan cleanupDelay = await _clock.WaitForDelayAsync();
        DateTime firstCleanupTime = _clock.GetUtcNow().UtcDateTime;

        // Assert
        Assert.Equal(_options.CleanupInterval, cleanupDelay);
        _scopeFactory.Verify(factory => factory.CreateScope(), Times.Once);

        // Act
        _clock.Advance(cleanupDelay);
        await _clock.WaitForDelayAsync();

        // Assert
        DateTime secondCleanupTime = _clock.GetUtcNow().UtcDateTime;
        Assert.Equal(firstCleanupTime.Add(_options.CleanupInterval), secondCleanupTime);

        _inbox.Verify(cleaner => cleaner.CleanPayloadsAsync(firstCleanupTime.AddDays(-7), It.IsAny<CancellationToken>()), Times.Once);
        _inbox.Verify(cleaner => cleaner.CleanPayloadsAsync(secondCleanupTime.AddDays(-7), It.IsAny<CancellationToken>()), Times.Once);
        _outbox.Verify(cleaner => cleaner.CleanEntriesAsync(firstCleanupTime.AddDays(-7), It.IsAny<CancellationToken>()), Times.Once);
        _outbox.Verify(cleaner => cleaner.CleanEntriesAsync(secondCleanupTime.AddDays(-7), It.IsAny<CancellationToken>()), Times.Once);
        _scopeFactory.Verify(factory => factory.CreateScope(), Times.Exactly(2));

        Assert.Equal(2, _scopes.Count);
        Assert.NotSame(_scopes[0].Object, _scopes[1].Object);

        foreach (Mock<IServiceScope> scope in _scopes)
        {
            scope.As<IAsyncDisposable>().Verify(item => item.DisposeAsync(), Times.Once);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ExecuteAsync_WhenCleanupFails_ContinuesOnNextCycle(bool cancellationException)
    {
        // Arrange
        Exception failure = cancellationException
            ? new OperationCanceledException()
            : new InvalidOperationException("Cleanup failed.");
        _inbox.SetupSequence(cleaner => cleaner.CleanPayloadsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(failure)
            .ReturnsAsync(0);

        // Act
        TimeSpan cleanupDelay = await RunFirstCleanupAsync();

        // Assert
        Assert.Equal(_options.CleanupInterval, cleanupDelay);
        Assert.Single(_scopes).As<IAsyncDisposable>().Verify(scope => scope.DisposeAsync(), Times.Once);
        _outbox.VerifyNoOtherCalls();
        LogAssert.Logged(_logger, LogLevel.Error, 2201, "RetentionCleanupFailed", failure, ("RetryDelay", _options.CleanupInterval));

        // Act
        _clock.Advance(cleanupDelay);
        await _clock.WaitForDelayAsync();

        // Assert
        _inbox.Verify(cleaner => cleaner.CleanPayloadsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _outbox.Verify(cleaner => cleaner.CleanEntriesAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.Equal(2, _scopes.Count);
        _scopes[1].As<IAsyncDisposable>().Verify(scope => scope.DisposeAsync(), Times.Once);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task StopAsync_DuringDelay_StopsWithoutFurtherCleanup(bool afterFirstCleanup)
    {
        // Arrange
        await _sut.StartAsync(TestContext.Current.CancellationToken);
        TimeSpan delay = await _clock.WaitForDelayAsync();
        if (afterFirstCleanup)
        {
            _clock.Advance(delay);
            await _clock.WaitForDelayAsync();
        }

        // Act
        await _sut.StopAsync(TestContext.Current.CancellationToken)
            .WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        await _sut.ExecuteTask!.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        _clock.Advance(TimeSpan.FromDays(1));

        // Assert
        Assert.True(_sut.ExecuteTask.IsCompletedSuccessfully);
        _scopeFactory.Verify(factory => factory.CreateScope(), Times.Exactly(afterFirstCleanup ? 1 : 0));
        _inbox.Verify(cleaner => cleaner.CleanPayloadsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Exactly(afterFirstCleanup ? 1 : 0));
        _outbox.Verify(cleaner => cleaner.CleanEntriesAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Exactly(afterFirstCleanup ? 1 : 0));
    }

    [Fact]
    public async Task StopAsync_DuringCleanup_CancelsCleanerAndDisposesScope()
    {
        // Arrange
        var cleanupStarted = new TaskCompletionSource<CancellationToken>(TaskCreationOptions.RunContinuationsAsynchronously);

        _inbox
            .Setup(cleaner => cleaner.CleanPayloadsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .Returns(async (DateTime _, CancellationToken token) =>
            {
                cleanupStarted.SetResult(token);
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
                return 0;
            });

        await _sut.StartAsync(TestContext.Current.CancellationToken);
        _clock.Advance(await _clock.WaitForDelayAsync());

        CancellationToken cleanerToken = await cleanupStarted.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        // Act
        await _sut.StopAsync(TestContext.Current.CancellationToken).WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        await _sut.ExecuteTask!.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        // Assert
        Assert.True(cleanerToken.IsCancellationRequested);
        Assert.True(_sut.ExecuteTask.IsCompletedSuccessfully);
        Assert.Single(_scopes).As<IAsyncDisposable>().Verify(scope => scope.DisposeAsync(), Times.Once);
        _outbox.VerifyNoOtherCalls();
        _logger.Verify(item => item.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
    }

    private void VerifyCleanupLog(string store, string operation, int affectedCount, DateTime cutoff)
    {
        LogAssert.Logged(
            _logger,
            LogLevel.Information,
            2202,
            "RetentionCleanupAffectedEntries",
            null,
            ("Store", store),
            ("Operation", operation),
            ("AffectedCount", affectedCount),
            ("Cutoff", cutoff));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(120)]
    public async Task ExecuteAsync_UsesConfiguredInitialDelay(int seconds)
    {
        // Arrange
        _options.InitialDelay = TimeSpan.FromSeconds(seconds);
        TimeSpan? initialDelay = null;
        int? scopeCountBeforeCleanup = null;

        // Act
        await _sut.StartAsync(TestContext.Current.CancellationToken);
        if (seconds > 0)
        {
            initialDelay = await _clock.WaitForDelayAsync();
            scopeCountBeforeCleanup = _scopeFactory.Invocations.Count;
            _clock.Advance(_options.InitialDelay);
        }
        TimeSpan cleanupDelay = await _clock.WaitForDelayAsync();

        // Assert
        if (seconds > 0)
        {
            Assert.Equal(_options.InitialDelay, initialDelay);
            Assert.Equal(0, scopeCountBeforeCleanup);
        }
        Assert.Equal(_options.CleanupInterval, cleanupDelay);
        _scopeFactory.Verify(factory => factory.CreateScope(), Times.Once);
    }

    private IServiceScope CreateScope()
    {
        var provider = new Mock<IServiceProvider>();
        provider.Setup(services => services.GetService(typeof(IInboxCleaner))).Returns(_inbox.Object);
        provider.Setup(services => services.GetService(typeof(IOutboxCleaner))).Returns(_outbox.Object);

        var scope = new Mock<IServiceScope>();
        scope.SetupGet(item => item.ServiceProvider).Returns(provider.Object);
        scope.As<IAsyncDisposable>().Setup(item => item.DisposeAsync()).Returns(ValueTask.CompletedTask);
        _scopes.Add(scope);

        return scope.Object;
    }

    private async Task<TimeSpan> RunFirstCleanupAsync()
    {
        await _sut.StartAsync(TestContext.Current.CancellationToken);
        _clock.Advance(await _clock.WaitForDelayAsync());
        return await _clock.WaitForDelayAsync();
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await _sut.StopAsync(timeout.Token);
            if (_sut.ExecuteTask is Task execution)
            {
                await execution.WaitAsync(timeout.Token);
            }
        }
        finally
        {
            _sut.Dispose();
        }
    }

    private sealed class TestTimeProvider() : FakeTimeProvider(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero))
    {
        private readonly Channel<TimeSpan> _delays = Channel.CreateUnbounded<TimeSpan>();

        public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
        {
            ITimer timer = base.CreateTimer(callback, state, dueTime, period);
            _delays.Writer.TryWrite(dueTime);
            return timer;
        }

        public async Task<TimeSpan> WaitForDelayAsync() => await _delays.Reader
            .ReadAsync(TestContext.Current.CancellationToken)
            .AsTask()
            .WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
    }
}
