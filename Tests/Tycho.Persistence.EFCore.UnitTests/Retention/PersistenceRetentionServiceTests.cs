using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Retention;
using Tycho.Persistence.EFCore.TestUtils.Logging;
using Tycho.Persistence.EFCore.TestUtils.Other;

namespace Tycho.Persistence.EFCore.UnitTests.Retention;

public sealed class PersistenceRetentionServiceTests : IAsyncLifetime
{
    private readonly TestTimeProvider _clock = new();
    private readonly Mock<IServiceScopeFactory> _scopeFactory = new();
    private readonly Mock<ILogger<PersistenceRetentionService>> _logger = new();
    private readonly List<Mock<IServiceScope>> _scopes = [];
    private readonly Mock<TychoDbContext> _context = new(new DbContextOptionsBuilder().Options);
    private readonly PersistenceRetentionSettings _options = new()
    {
        Inbox = new() { PayloadRetention = null, FullCleanupRetention = null },
        Outbox = new() { PayloadRetention = null, FullCleanupRetention = null }
    };
    private PersistenceRetentionService _sut = null!;

    public ValueTask InitializeAsync()
    {
        var services = new Mock<IServiceProvider>();
        services.Setup(p => p.GetService(typeof(TychoDbContext))).Returns(_context.Object);
        services.Setup(p => p.GetService(typeof(PersistenceOwner)))
            .Returns(new PersistenceOwner(PersistenceTestInternals.Create(typeof(PersistenceOwner))));
        _scopeFactory.Setup(f => f.CreateScope()).Returns(() =>
        {
            var scope = new Mock<IServiceScope>();
            scope.SetupGet(s => s.ServiceProvider).Returns(services.Object);
            scope.As<IAsyncDisposable>().Setup(s => s.DisposeAsync()).Returns(ValueTask.CompletedTask);
            _scopes.Add(scope);
            return scope.Object;
        });
        _logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _sut = new PersistenceRetentionService(_scopeFactory.Object, _options, _clock, _logger.Object);
        return ValueTask.CompletedTask;
    }

    [Fact]
    public async Task ExecuteAsync_WithInitialDelay_WaitsBeforeStartingCleanup()
    {
        // Arrange
        _options.InitialDelay = TimeSpan.FromSeconds(120);

        // Act
        await _sut.StartAsync(TestContext.Current.CancellationToken);
        Assert.Equal(_options.InitialDelay, await _clock.NextDelayAsync());
        Assert.Empty(_scopes);
        _clock.Advance(_options.InitialDelay);

        // Assert
        Assert.Equal(_options.CleanupInterval, await _clock.NextDelayAsync());
        Assert.Single(_scopes).As<IAsyncDisposable>().Verify(s => s.DisposeAsync(), Times.Once);
        _clock.Advance(_options.CleanupInterval);
        Assert.Equal(_options.CleanupInterval, await _clock.NextDelayAsync());
        Assert.Equal(2, _scopes.Count);
        Assert.NotSame(_scopes[0].Object, _scopes[1].Object);
        _scopes[1].As<IAsyncDisposable>().Verify(s => s.DisposeAsync(), Times.Once);
        _context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithZeroInitialDelay_StartsCleanupImmediately()
    {
        // Arrange
        _options.InitialDelay = TimeSpan.Zero;

        // Act
        await _sut.StartAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(_options.CleanupInterval, await _clock.NextDelayAsync());
        Assert.Single(_scopes).As<IAsyncDisposable>().Verify(s => s.DisposeAsync(), Times.Once);
        _clock.Advance(_options.CleanupInterval);
        Assert.Equal(_options.CleanupInterval, await _clock.NextDelayAsync());
        Assert.Equal(2, _scopes.Count);
        Assert.NotSame(_scopes[0].Object, _scopes[1].Object);
        _scopes[1].As<IAsyncDisposable>().Verify(s => s.DisposeAsync(), Times.Once);
        _context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnrelatedCancellation_LogsFailureAndRetries()
    {
        // Arrange
        Exception error = new OperationCanceledException();
        _scopeFactory.SetupSequence(f => f.CreateScope()).Throws(error).Returns(() =>
        {
            var scope = new Mock<IServiceScope>();
            var services = new Mock<IServiceProvider>();
            services.Setup(p => p.GetService(typeof(TychoDbContext))).Returns(_context.Object);
            services.Setup(p => p.GetService(typeof(PersistenceOwner)))
                .Returns(new PersistenceOwner(PersistenceTestInternals.Create(typeof(PersistenceOwner))));
            scope.SetupGet(s => s.ServiceProvider).Returns(services.Object);
            scope.As<IAsyncDisposable>().Setup(s => s.DisposeAsync()).Returns(ValueTask.CompletedTask);
            _scopes.Add(scope);
            return scope.Object;
        });

        // Act
        await _sut.StartAsync(TestContext.Current.CancellationToken);
        _clock.Advance(await _clock.NextDelayAsync());

        // Assert
        Assert.Equal(_options.CleanupInterval, await _clock.NextDelayAsync());
        LogAssert.Logged(_logger, LogLevel.Error, 2201, "RetentionCleanupFailed", error, ("RetryDelay", _options.CleanupInterval));
        _clock.Advance(_options.CleanupInterval);
        await _clock.NextDelayAsync();
        _scopeFactory.Verify(f => f.CreateScope(), Times.Exactly(2));
        Assert.Single(_scopes).As<IAsyncDisposable>().Verify(s => s.DisposeAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithScopeCreationFailure_LogsFailureAndRetries()
    {
        // Arrange
        Exception error = new InvalidOperationException("scope failure");
        _scopeFactory.SetupSequence(f => f.CreateScope()).Throws(error).Returns(() =>
        {
            var scope = new Mock<IServiceScope>();
            var services = new Mock<IServiceProvider>();
            services.Setup(p => p.GetService(typeof(TychoDbContext))).Returns(_context.Object);
            services.Setup(p => p.GetService(typeof(PersistenceOwner)))
                .Returns(new PersistenceOwner(PersistenceTestInternals.Create(typeof(PersistenceOwner))));
            scope.SetupGet(s => s.ServiceProvider).Returns(services.Object);
            scope.As<IAsyncDisposable>().Setup(s => s.DisposeAsync()).Returns(ValueTask.CompletedTask);
            _scopes.Add(scope);
            return scope.Object;
        });

        // Act
        await _sut.StartAsync(TestContext.Current.CancellationToken);
        _clock.Advance(await _clock.NextDelayAsync());

        // Assert
        Assert.Equal(_options.CleanupInterval, await _clock.NextDelayAsync());
        LogAssert.Logged(_logger, LogLevel.Error, 2201, "RetentionCleanupFailed", error, ("RetryDelay", _options.CleanupInterval));
        _clock.Advance(_options.CleanupInterval);
        await _clock.NextDelayAsync();
        _scopeFactory.Verify(f => f.CreateScope(), Times.Exactly(2));
        Assert.Single(_scopes).As<IAsyncDisposable>().Verify(s => s.DisposeAsync(), Times.Once);
    }

    [Fact]
    public async Task StopAsync_DuringCleanupDelay_StopsWithoutFurtherCycles()
    {
        // Arrange
        await _sut.StartAsync(TestContext.Current.CancellationToken);
        TimeSpan delay = await _clock.NextDelayAsync();
        _clock.Advance(delay);
        await _clock.NextDelayAsync();

        // Act
        await _sut.StopAsync(TestContext.Current.CancellationToken);
        await _sut.ExecuteTask!.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        _clock.Advance(TimeSpan.FromDays(1));

        // Assert
        Assert.True(_sut.ExecuteTask.IsCompletedSuccessfully);
        _scopeFactory.Verify(f => f.CreateScope(), Times.Exactly(1));
    }

    [Fact]
    public async Task StopAsync_DuringInitialDelay_StopsWithoutCleanup()
    {
        // Arrange
        await _sut.StartAsync(TestContext.Current.CancellationToken);
        TimeSpan delay = await _clock.NextDelayAsync();

        // Act
        await _sut.StopAsync(TestContext.Current.CancellationToken);
        await _sut.ExecuteTask!.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        _clock.Advance(TimeSpan.FromDays(1));

        // Assert
        Assert.True(_sut.ExecuteTask.IsCompletedSuccessfully);
        _scopeFactory.Verify(f => f.CreateScope(), Times.Exactly(0));
    }

    public async ValueTask DisposeAsync()
    {
        if (_sut is not null)
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await _sut.StopAsync(timeout.Token);
            _sut.Dispose();
        }
        await _context.Object.DisposeAsync();
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

        public async Task<TimeSpan> NextDelayAsync() => await _delays.Reader.ReadAsync(TestContext.Current.CancellationToken)
            .AsTask().WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
    }
}
