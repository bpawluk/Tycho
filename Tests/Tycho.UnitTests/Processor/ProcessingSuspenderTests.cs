using Tycho.Processor;

namespace Tycho.UnitTests.Processor;

public sealed class ProcessingSuspenderTests
{
    private static readonly TimeSpan s_waitTimeout = TimeSpan.FromSeconds(5);

    [Fact]
    public async Task SuspendAsync_ZeroDuration_ReturnsCompleted()
    {
        // Arrange
        var sut = new ProcessingSuspender();

        // Act
        SuspendResult result = await sut.SuspendAsync(TimeSpan.Zero, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(SuspendResult.Completed, result);
    }

    [Fact]
    public async Task SuspendAsync_WhenResumed_ReturnsInterrupted()
    {
        // Arrange
        var sut = new ProcessingSuspender();
        using var cleanup = new CancellationTokenSource();

        Task<SuspendResult> suspension = sut.SuspendAsync(Timeout.InfiniteTimeSpan, cleanup.Token);

        try
        {
            // Act
            sut.TryResume();
            SuspendResult result = await suspension.WaitAsync(s_waitTimeout, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(SuspendResult.Interrupted, result);
        }
        finally
        {
            cleanup.Cancel();
        }
    }

    [Fact]
    public async Task SuspendAsync_WhenCallerCancels_PropagatesCancellation()
    {
        // Arrange
        var sut = new ProcessingSuspender();
        using var callerCancellation = new CancellationTokenSource();

        Task<SuspendResult> suspension = sut.SuspendAsync(Timeout.InfiniteTimeSpan, callerCancellation.Token);

        // Act
        callerCancellation.Cancel();

        // Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => suspension.WaitAsync(s_waitTimeout, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task SuspendAsync_WhenAlreadySuspended_RejectsSecondSuspension()
    {
        // Arrange
        var sut = new ProcessingSuspender();
        using var cleanup = new CancellationTokenSource();

        Task<SuspendResult> first = sut.SuspendAsync(Timeout.InfiniteTimeSpan, cleanup.Token);

        try
        {
            // Act
            InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.SuspendAsync(TimeSpan.Zero, CancellationToken.None));

            // Assert
            Assert.Equal("Only one suspension can be active at a time.", exception.Message);
            Assert.False(first.IsCompleted);
        }
        finally
        {
            sut.TryResume();
            cleanup.Cancel();
            try
            {
                await first.WaitAsync(s_waitTimeout, TestContext.Current.CancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Caller cancellation also releases the active suspension.
            }
        }
    }

    [Fact]
    public async Task SuspendAsync_AfterCompletion_AllowsAnotherSuspension()
    {
        // Arrange
        var sut = new ProcessingSuspender();

        await sut.SuspendAsync(TimeSpan.Zero, TestContext.Current.CancellationToken);

        // Act
        SuspendResult result = await sut.SuspendAsync(TimeSpan.Zero, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(SuspendResult.Completed, result);
    }

    [Fact]
    public async Task SuspendAsync_AfterResume_AllowsAnotherSuspension()
    {
        // Arrange
        var sut = new ProcessingSuspender();
        using var cleanup = new CancellationTokenSource();

        Task<SuspendResult> first = sut.SuspendAsync(Timeout.InfiniteTimeSpan, cleanup.Token);

        try
        {
            sut.TryResume();
            await first.WaitAsync(s_waitTimeout, TestContext.Current.CancellationToken);

            // Act
            SuspendResult result = await sut.SuspendAsync(TimeSpan.Zero, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(SuspendResult.Completed, result);
        }
        finally
        {
            cleanup.Cancel();
        }
    }

    [Fact]
    public async Task SuspendAsync_AfterCallerCancellation_AllowsAnotherSuspension()
    {
        // Arrange
        var sut = new ProcessingSuspender();
        using var callerCancellation = new CancellationTokenSource();

        Task<SuspendResult> first = sut.SuspendAsync(Timeout.InfiniteTimeSpan, callerCancellation.Token);
        callerCancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => first.WaitAsync(s_waitTimeout, TestContext.Current.CancellationToken));

        // Act
        SuspendResult result = await sut.SuspendAsync(TimeSpan.Zero, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(SuspendResult.Completed, result);
    }

    [Fact]
    public async Task SuspendAsync_AfterInvalidDuration_AllowsAnotherSuspension()
    {
        // Arrange
        var sut = new ProcessingSuspender();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => sut.SuspendAsync(TimeSpan.FromMilliseconds(-2), CancellationToken.None));

        // Act
        SuspendResult result = await sut.SuspendAsync(TimeSpan.Zero, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(SuspendResult.Completed, result);
    }

    [Fact]
    public void TryResume_WithoutActiveSuspension_DoesNotThrow()
    {
        // Arrange
        var sut = new ProcessingSuspender();

        // Act
        sut.TryResume();
    }
}
