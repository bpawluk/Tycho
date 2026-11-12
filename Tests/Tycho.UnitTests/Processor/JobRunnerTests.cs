using System.Collections.Concurrent;
using Tycho.Processor;

namespace Tycho.UnitTests.Processor;

public sealed class JobRunnerTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WhenConcurrencyIsNotPositive_Throws(int maximalConcurrency)
    {
        // Act
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new JobRunner(maximalConcurrency, TimeSpan.FromSeconds(1), _ => { }));

        // Assert
        Assert.Equal("maximalConcurrency", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void Constructor_WhenTimeoutIsInvalid_Throws(int timeoutMilliseconds)
    {
        // Arrange
        TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

        // Act
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new JobRunner(1, timeout, _ => { }));

        // Assert
        Assert.Equal("jobTimeout", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenErrorHandlerIsNull_Throws()
    {
        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
            () => new JobRunner(1, TimeSpan.FromSeconds(1), null!));

        // Assert
        Assert.Equal("onError", exception.ParamName);
    }

    [Fact(Timeout = 10_000)]
    public async Task Constructor_WithInfiniteTimeout_AcceptsAndRunsJob()
    {
        // Arrange
        var errors = new ErrorRecorder();
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, errors.Record);
        var job = new ControlledJob();

        try
        {
            // Act
            runner.Run(job);
            CancellationToken jobToken = await job.Started.WaitAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.False(jobToken.IsCancellationRequested);
        }
        finally
        {
            await CompleteAndStopAsync(runner, job);
        }

        Assert.Equal(0, errors.Count);
    }

    [Fact(Timeout = 10_000)]
    public async Task WaitForCapacityAsync_WhenAvailable_ReturnsWithoutReservingSlot()
    {
        // Arrange
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, _ => { });
        var job = new ControlledJob();

        try
        {
            // Act
            await runner.WaitForCapacityAsync(TestContext.Current.CancellationToken);
            runner.Run(job);

            // Assert
            await job.Started.WaitAsync(TestContext.Current.CancellationToken);
        }
        finally
        {
            await CompleteAndStopAsync(runner, job);
        }
    }

    [Fact(Timeout = 10_000)]
    public async Task WaitForCapacityAsync_WhenFull_WaitsForJobCompletion()
    {
        // Arrange
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, _ => { });
        var job = new ControlledJob();
        runner.Run(job);

        try
        {
            await job.Started.WaitAsync(TestContext.Current.CancellationToken);
            Task wait = runner.WaitForCapacityAsync(TestContext.Current.CancellationToken);
            Assert.False(wait.IsCompleted);

            // Act
            job.Complete();
            await wait;
        }
        finally
        {
            await CompleteAndStopAsync(runner, job);
        }
    }

    [Fact(Timeout = 10_000)]
    public async Task WaitForCapacityAsync_WhenWaitingIsCanceled_DoesNotConsumeCapacity()
    {
        // Arrange
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, _ => { });
        using var waitCancellation = new CancellationTokenSource();
        var job = new ControlledJob();
        runner.Run(job);

        try
        {
            await job.Started.WaitAsync(TestContext.Current.CancellationToken);
            Task wait = runner.WaitForCapacityAsync(waitCancellation.Token);
            Assert.False(wait.IsCompleted);

            // Act
            waitCancellation.Cancel();

            // Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => wait);
            job.Complete();
            await runner.WaitForCapacityAsync(TestContext.Current.CancellationToken);
        }
        finally
        {
            await CompleteAndStopAsync(runner, job);
        }
    }

    [Fact]
    public async Task WaitForCapacityAsync_WithAlreadyCanceledToken_Throws()
    {
        // Arrange
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, _ => { });
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        // Act and Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => runner.WaitForCapacityAsync(cancellation.Token));
    }

    [Fact]
    public void Run_WhenJobIsNull_Throws()
    {
        // Arrange
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, _ => { });

        // Act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => runner.Run(null!));

        // Assert
        Assert.Equal("job", exception.ParamName);
    }

    [Fact(Timeout = 10_000)]
    public async Task Run_WhenFull_RejectsJobWithoutStartingIt()
    {
        // Arrange
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, _ => { });
        var first = new ControlledJob();
        var rejected = new ControlledJob();
        runner.Run(first);

        try
        {
            await first.Started.WaitAsync(TestContext.Current.CancellationToken);

            // Act
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => runner.Run(rejected));

            // Assert
            Assert.Equal("No job execution capacity is available.", exception.Message);
            Assert.False(rejected.Started.IsCompleted);
        }
        finally
        {
            await CompleteAndStopAsync(runner, first, rejected);
        }
    }

    [Fact(Timeout = 10_000)]
    public async Task Run_WithTwoSlots_AcceptsAnotherJobAfterOneFinishes()
    {
        // Arrange
        using var runner = new JobRunner(2, Timeout.InfiniteTimeSpan, _ => { });
        var first = new ControlledJob();
        var second = new ControlledJob();
        var third = new ControlledJob();
        runner.Run(first);
        runner.Run(second);

        try
        {
            await first.Started.WaitAsync(TestContext.Current.CancellationToken);
            await second.Started.WaitAsync(TestContext.Current.CancellationToken);
            Assert.Throws<InvalidOperationException>(() => runner.Run(third));

            // Act
            first.Complete();
            await runner.WaitForCapacityAsync(TestContext.Current.CancellationToken);
            runner.Run(third);

            // Assert
            await third.Started.WaitAsync(TestContext.Current.CancellationToken);
        }
        finally
        {
            await CompleteAndStopAsync(runner, first, second, third);
        }
    }

    [Fact(Timeout = 10_000)]
    public async Task Run_AfterSuccessfulJob_ReleasesCapacity()
    {
        // Arrange
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, _ => { });
        var first = new ControlledJob();
        var second = new ControlledJob();
        runner.Run(first);

        try
        {
            await first.Started.WaitAsync(TestContext.Current.CancellationToken);

            // Act
            first.Complete();
            await runner.WaitForCapacityAsync(TestContext.Current.CancellationToken);
            runner.Run(second);

            // Assert
            await second.Started.WaitAsync(TestContext.Current.CancellationToken);
        }
        finally
        {
            await CompleteAndStopAsync(runner, first, second);
        }
    }

    [Fact(Timeout = 10_000)]
    public async Task Run_WhenJobThrowsSynchronously_ReportsOriginalExceptionAndReleasesCapacity()
    {
        // Arrange
        var errors = new ErrorRecorder();
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, errors.Record);
        var failure = new InvalidOperationException("synchronous failure");
        var job = new DelegateJob(_ => throw failure);

        // Act
        runner.Run(job);
        Exception reported = await errors.First.WaitAsync(TestContext.Current.CancellationToken);
        await runner.WaitForCapacityAsync(TestContext.Current.CancellationToken);
        await runner.StopAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(failure, reported);
        Assert.Equal(1, errors.Count);
    }

    [Fact(Timeout = 10_000)]
    public async Task Run_WhenJobFailsAsynchronously_ReportsOriginalExceptionAndReleasesCapacity()
    {
        // Arrange
        var errors = new ErrorRecorder();
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, errors.Record);
        var job = new ControlledJob();
        var failure = new InvalidOperationException("asynchronous failure");
        runner.Run(job);

        try
        {
            await job.Started.WaitAsync(TestContext.Current.CancellationToken);

            // Act
            job.Fail(failure);
            Exception reported = await errors.First.WaitAsync(TestContext.Current.CancellationToken);
            await runner.WaitForCapacityAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.Same(failure, reported);
        }
        finally
        {
            await CompleteAndStopAsync(runner, job);
        }

        Assert.Equal(1, errors.Count);
    }

    [Fact(Timeout = 10_000)]
    public async Task Run_WhenJobCancelsItself_ReportsCancellationAsError()
    {
        // Arrange
        var errors = new ErrorRecorder();
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, errors.Record);
        var job = new ControlledJob();
        var failure = new OperationCanceledException("job canceled itself");
        runner.Run(job);

        try
        {
            await job.Started.WaitAsync(TestContext.Current.CancellationToken);

            // Act
            job.Fail(failure);
            Exception reported = await errors.First.WaitAsync(TestContext.Current.CancellationToken);
            await runner.WaitForCapacityAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.Same(failure, reported);
        }
        finally
        {
            await CompleteAndStopAsync(runner, job);
        }

        Assert.Equal(1, errors.Count);
    }

    [Fact(Timeout = 10_000)]
    public async Task Run_WhenErrorHandlerThrows_CompletesJobAndReleasesCapacity()
    {
        // Arrange
        var errors = new ErrorRecorder();
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, error =>
        {
            errors.Record(error);
            throw new InvalidOperationException("error handler failure");
        });
        var failure = new InvalidOperationException("job failure");

        // Act
        runner.Run(new DelegateJob(_ => Task.FromException(failure)));
        await errors.First.WaitAsync(TestContext.Current.CancellationToken);
        await runner.WaitForCapacityAsync(TestContext.Current.CancellationToken);
        await runner.StopAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(1, errors.Count);
    }

    [Fact(Timeout = 10_000)]
    public async Task Run_WhenJobTimesOut_ReportsTimeoutAndReleasesCapacity()
    {
        // Arrange
        var errors = new ErrorRecorder();
        using var runner = new JobRunner(1, TimeSpan.FromMilliseconds(100), errors.Record);
        var job = new ControlledJob();
        runner.Run(job);

        try
        {
            CancellationToken jobToken = await job.Started.WaitAsync(TestContext.Current.CancellationToken);

            // Act
            Exception reported = await errors.First.WaitAsync(TestContext.Current.CancellationToken);
            await runner.WaitForCapacityAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.IsType<TimeoutException>(reported);
            Assert.True(jobToken.IsCancellationRequested);
        }
        finally
        {
            await CompleteAndStopAsync(runner, job);
        }

        Assert.Equal(1, errors.Count);
    }

    [Fact(Timeout = 10_000)]
    public async Task StopAsync_WithNoJobs_Completes()
    {
        // Arrange
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, _ => { });

        // Act
        await runner.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact(Timeout = 10_000)]
    public async Task StopAsync_WithCooperativeJob_CancelsWithoutReportingError()
    {
        // Arrange
        var errors = new ErrorRecorder();
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, errors.Record);
        var job = new ControlledJob();
        runner.Run(job);

        try
        {
            CancellationToken jobToken = await job.Started.WaitAsync(TestContext.Current.CancellationToken);

            // Act
            await runner.StopAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.True(jobToken.IsCancellationRequested);
            Assert.Equal(0, errors.Count);
        }
        finally
        {
            await CompleteAndStopAsync(runner, job);
        }
    }

    [Fact(Timeout = 10_000)]
    public async Task StopAsync_WithUncooperativeJob_WaitsForCompletion()
    {
        // Arrange
        var errors = new ErrorRecorder();
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, errors.Record);
        var job = new ControlledJob(observeCancellation: false);
        runner.Run(job);

        try
        {
            CancellationToken jobToken = await job.Started.WaitAsync(TestContext.Current.CancellationToken);
            var canceled = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using CancellationTokenRegistration registration = jobToken.Register(() => canceled.TrySetResult(true));

            // Act
            Task stop = runner.StopAsync(CancellationToken.None);
            await canceled.Task.WaitAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.False(stop.IsCompleted);
            job.Complete();
            await stop.WaitAsync(TestContext.Current.CancellationToken);
            Assert.Equal(0, errors.Count);
        }
        finally
        {
            await CompleteAndStopAsync(runner, job);
        }
    }

    [Fact(Timeout = 10_000)]
    public async Task StopAsync_WhenCallerCancelsWait_UnderlyingStopContinues()
    {
        // Arrange
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, _ => { });
        using var waitCancellation = new CancellationTokenSource();
        var job = new ControlledJob(observeCancellation: false);
        runner.Run(job);

        try
        {
            CancellationToken jobToken = await job.Started.WaitAsync(TestContext.Current.CancellationToken);
            var canceled = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using CancellationTokenRegistration registration = jobToken.Register(() => canceled.TrySetResult(true));
            Task firstStop = runner.StopAsync(waitCancellation.Token);
            await canceled.Task.WaitAsync(TestContext.Current.CancellationToken);

            // Act
            waitCancellation.Cancel();

            // Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => firstStop);
            Task continuedStop = runner.StopAsync(CancellationToken.None);
            Assert.False(continuedStop.IsCompleted);
            job.Complete();
            await continuedStop.WaitAsync(TestContext.Current.CancellationToken);
        }
        finally
        {
            await CompleteAndStopAsync(runner, job);
        }
    }

    [Fact(Timeout = 10_000)]
    public async Task StopAsync_WhenCalledTwice_BothCallsWaitForActiveJobs()
    {
        // Arrange
        using var runner = new JobRunner(1, Timeout.InfiniteTimeSpan, _ => { });
        var job = new ControlledJob(observeCancellation: false);
        runner.Run(job);

        try
        {
            CancellationToken jobToken = await job.Started.WaitAsync(TestContext.Current.CancellationToken);
            var canceled = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using CancellationTokenRegistration registration = jobToken.Register(() => canceled.TrySetResult(true));

            // Act
            Task firstStop = runner.StopAsync(CancellationToken.None);
            Task secondStop = runner.StopAsync(CancellationToken.None);
            await canceled.Task.WaitAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.False(firstStop.IsCompleted);
            Assert.False(secondStop.IsCompleted);
            job.Complete();
            await firstStop.WaitAsync(TestContext.Current.CancellationToken);
            await secondStop.WaitAsync(TestContext.Current.CancellationToken);
        }
        finally
        {
            await CompleteAndStopAsync(runner, job);
        }
    }

    private static async Task CompleteAndStopAsync(JobRunner runner, params ControlledJob[] jobs)
    {
        foreach (ControlledJob job in jobs)
        {
            job.Complete();
        }
        await runner.StopAsync(TestContext.Current.CancellationToken);
    }

    private sealed class ControlledJob(bool observeCancellation = true) : IJob
    {
        private readonly bool _observeCancellation = observeCancellation;

        private readonly TaskCompletionSource<CancellationToken> _started =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        private readonly TaskCompletionSource<bool> _completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task<CancellationToken> Started => _started.Task;

        public void Complete() => _completion.TrySetResult(true);

        public void Fail(Exception exception) => _completion.TrySetException(exception);

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _started.TrySetResult(cancellationToken);
            return _observeCancellation
                ? _completion.Task.WaitAsync(cancellationToken)
                : _completion.Task;
        }
    }

    private sealed class DelegateJob(Func<CancellationToken, Task> execute) : IJob
    {
        private readonly Func<CancellationToken, Task> _execute = execute;

        public Task ExecuteAsync(CancellationToken cancellationToken) => _execute(cancellationToken);
    }

    private sealed class ErrorRecorder
    {
        private readonly ConcurrentQueue<Exception> _errors = new();

        private readonly TaskCompletionSource<Exception> _first =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task<Exception> First => _first.Task;

        public int Count => _errors.Count;

        public void Record(Exception exception)
        {
            _errors.Enqueue(exception);
            _first.TrySetResult(exception);
        }
    }
}
