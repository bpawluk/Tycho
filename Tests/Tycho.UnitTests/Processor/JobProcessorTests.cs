using System.Collections.Concurrent;
using System.Threading.Channels;
using Tycho.Processor;

namespace Tycho.UnitTests.Processor;

public sealed class JobProcessorTests
{
    [Fact(Timeout = 10_000)]
    public async Task Start_BeginsWaitingForRunnerCapacity()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        // Act
        harness.Processor.Start();
        CancellationToken processingToken = await harness.Runner.NextCapacityWaitAsync(cancellationToken);

        // Assert
        Assert.True(processingToken.CanBeCanceled);
        Assert.False(processingToken.IsCancellationRequested);
    }

    [Fact(Timeout = 10_000)]
    public async Task Start_WhenAlreadyStarted_Throws()
    {
        // Arrange
        await using var harness = new ProcessorHarness();

        harness.Processor.Start();
        await harness.Runner.NextCapacityWaitAsync(TestContext.Current.CancellationToken);

        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(harness.Processor.Start);

        // Assert
        Assert.Equal("Processing already started.", exception.Message);
    }

    [Fact]
    public void Ping_BeforeStart_Throws()
    {
        // Arrange
        using var harness = new ProcessorHarness();

        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(harness.Processor.Ping);

        // Assert
        Assert.Equal("Processing is not running.", exception.Message);
    }

    [Fact(Timeout = 10_000)]
    public async Task Ping_WhileRunning_ResumesSuspension()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        harness.Factory.EnqueueJob(null);
        harness.Processor.Start();
        await harness.Suspender.NextSuspensionAsync(cancellationToken);

        // Act
        harness.Processor.Ping();

        // Assert
        await harness.Suspender.NextResumeAsync(cancellationToken);
    }

    [Fact(Timeout = 10_000)]
    public async Task Ping_AfterStop_Throws()
    {
        // Arrange
        await using var harness = new ProcessorHarness();

        harness.Processor.Start();
        await harness.Runner.NextCapacityWaitAsync(TestContext.Current.CancellationToken);
        await harness.Processor.StopAsync(TestContext.Current.CancellationToken);

        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(harness.Processor.Ping);

        // Assert
        Assert.Equal("Processing was stopped.", exception.Message);
    }

    [Fact(Timeout = 10_000)]
    public async Task Start_AfterStop_Throws()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        await harness.Processor.StopAsync(TestContext.Current.CancellationToken);

        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(harness.Processor.Start);

        // Assert
        Assert.Equal("Processing was stopped.", exception.Message);
    }

    [Fact(Timeout = 10_000)]
    public async Task StopAsync_BeforeStart_StopsRunner()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        // Act
        await harness.Processor.StopAsync(cancellationToken);

        // Assert
        Assert.Equal(cancellationToken, await harness.Runner.NextStopAsync(cancellationToken));
        Assert.Equal(0, harness.Factory.CallCount);
    }

    [Fact]
    public void Dispose_DisposesRunner()
    {
        // Arrange
        var harness = new ProcessorHarness();

        // Act
        harness.Processor.Dispose();

        // Assert
        Assert.True(harness.Runner.IsDisposed);
    }

    [Fact(Timeout = 10_000)]
    public async Task ProcessIteration_WaitsForCapacityBeforeCallingFactory()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        TaskCompletionSource<bool> capacity = NewSignal();
        harness.Runner.WaitForCapacity = capacity.Task.WaitAsync;

        // Act
        harness.Processor.Start();
        await harness.Runner.NextCapacityWaitAsync(cancellationToken);

        // Assert
        Assert.Equal(0, harness.Factory.CallCount);

        // Act
        capacity.SetResult(true);
        await harness.Factory.NextCallAsync(cancellationToken);

        // Assert
        Assert.Equal(1, harness.Factory.CallCount);
    }

    [Fact(Timeout = 10_000)]
    public async Task ProcessIteration_WhenJobIsCreated_RunsSameJobAndResetsInterval()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var job = new SignalingJob();
        harness.Factory.EnqueueJob(job);

        // Act
        harness.Processor.Start();
        IJob runJob = await harness.Runner.NextRunAsync(cancellationToken);
        string intervalChange = await harness.Interval.NextChangeAsync(cancellationToken);

        // Assert
        Assert.Same(job, runJob);
        Assert.Equal("Reset", intervalChange);
        Assert.Equal(0, harness.Suspender.SuspendCount);
        await harness.Factory.NextCallAsync(cancellationToken);
        await harness.Factory.NextCallAsync(cancellationToken);
        Assert.Equal(2, harness.Factory.CallCount);
    }

    [Fact(Timeout = 10_000)]
    public async Task ProcessIteration_WhenNoJobIsCreated_SuspendsForCurrentInterval()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        harness.Factory.EnqueueJob(null);

        // Act
        harness.Processor.Start();
        TimeSpan duration = await harness.Suspender.NextSuspensionAsync(cancellationToken);

        // Assert
        Assert.Equal(TimeSpan.FromTicks(10), duration);
        Assert.Equal(0, harness.Runner.RunCount);
    }

    [Fact(Timeout = 10_000)]
    public async Task ProcessLoop_WhenSuspensionCompletes_IncreasesNextInterval()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        harness.Factory.EnqueueJob(null);
        harness.Factory.EnqueueJob(null);
        harness.Suspender.EnqueueResult(SuspendResult.Completed);

        // Act
        harness.Processor.Start();
        TimeSpan firstDuration = await harness.Suspender.NextSuspensionAsync(cancellationToken);
        string intervalChange = await harness.Interval.NextChangeAsync(cancellationToken);
        TimeSpan secondDuration = await harness.Suspender.NextSuspensionAsync(cancellationToken);

        // Assert
        Assert.Equal(TimeSpan.FromTicks(10), firstDuration);
        Assert.Equal("Increase", intervalChange);
        Assert.Equal(TimeSpan.FromTicks(20), secondDuration);
    }

    [Fact(Timeout = 10_000)]
    public async Task ProcessLoop_WhenSuspensionIsInterrupted_KeepsCurrentInterval()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        harness.Factory.EnqueueJob(null);
        harness.Factory.EnqueueJob(null);
        harness.Suspender.EnqueueResult(SuspendResult.Interrupted);

        // Act
        harness.Processor.Start();
        TimeSpan firstDuration = await harness.Suspender.NextSuspensionAsync(cancellationToken);
        TimeSpan secondDuration = await harness.Suspender.NextSuspensionAsync(cancellationToken);

        // Assert
        Assert.Equal(TimeSpan.FromTicks(10), firstDuration);
        Assert.Equal(TimeSpan.FromTicks(10), secondDuration);
        Assert.Equal(0, harness.Interval.IncreaseCount);
    }

    [Fact(Timeout = 10_000)]
    public async Task ProcessLoop_WhenJobFollowsIdleIteration_ResetsInterval()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var job = new SignalingJob();
        harness.Factory.EnqueueJob(null);
        harness.Factory.EnqueueJob(job);
        harness.Suspender.EnqueueResult(SuspendResult.Completed);

        // Act
        harness.Processor.Start();
        await harness.Interval.NextChangeAsync(cancellationToken);
        IJob runJob = await harness.Runner.NextRunAsync(cancellationToken);
        string intervalChange = await harness.Interval.NextChangeAsync(cancellationToken);

        // Assert
        Assert.Same(job, runJob);
        Assert.Equal("Reset", intervalChange);
        Assert.Equal(TimeSpan.FromTicks(10), harness.Interval.Current);
    }

    [Fact(Timeout = 10_000)]
    public async Task ProcessLoop_WhenCapacityWaitFails_ReportsErrorAndRetries()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var failure = new InvalidOperationException("capacity failed");
        int waits = 0;
        harness.Runner.WaitForCapacity = _ =>
            Interlocked.Increment(ref waits) == 1 ? Task.FromException(failure) : Task.CompletedTask;
        harness.Suspender.EnqueueResult(SuspendResult.Completed);

        // Act
        harness.Processor.Start();
        (object? sender, Exception reported) = await harness.NextErrorAsync(cancellationToken);
        await harness.Suspender.NextSuspensionAsync(cancellationToken);
        await harness.Factory.NextCallAsync(cancellationToken);

        // Assert
        Assert.Same(harness.Processor, sender);
        Assert.Same(failure, reported);
        Assert.Equal(1, harness.ErrorCount);
    }

    [Fact(Timeout = 10_000)]
    public async Task ProcessLoop_WhenFactoryFails_ReportsErrorAndRetries()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var failure = new InvalidOperationException("factory failed");
        harness.Factory.Enqueue(_ => Task.FromException<IJob?>(failure));
        harness.Suspender.EnqueueResult(SuspendResult.Completed);

        // Act
        harness.Processor.Start();
        (object? sender, Exception reported) = await harness.NextErrorAsync(cancellationToken);
        await harness.Suspender.NextSuspensionAsync(cancellationToken);
        await harness.Factory.NextCallAsync(cancellationToken);
        await harness.Factory.NextCallAsync(cancellationToken);

        // Assert
        Assert.Same(harness.Processor, sender);
        Assert.Same(failure, reported);
        Assert.Equal(1, harness.ErrorCount);
    }

    [Fact(Timeout = 10_000)]
    public async Task ProcessLoop_WhenRunnerRejectsJob_ReportsErrorAndRetries()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var failure = new InvalidOperationException("runner rejected job");
        var job = new SignalingJob();
        harness.Factory.EnqueueJob(job);
        harness.Runner.OnRun = _ => throw failure;
        harness.Suspender.EnqueueResult(SuspendResult.Completed);

        // Act
        harness.Processor.Start();
        (object? sender, Exception reported) = await harness.NextErrorAsync(cancellationToken);
        await harness.Suspender.NextSuspensionAsync(cancellationToken);
        await harness.Factory.NextCallAsync(cancellationToken);
        await harness.Factory.NextCallAsync(cancellationToken);

        // Assert
        Assert.Same(harness.Processor, sender);
        Assert.Same(failure, reported);
        Assert.Equal(1, harness.ErrorCount);
    }

    [Fact(Timeout = 10_000)]
    public async Task ProcessLoop_WhenErrorSubscriberThrows_ContinuesProcessing()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var failure = new InvalidOperationException("factory failed");
        harness.Factory.Enqueue(_ => Task.FromException<IJob?>(failure));
        harness.Suspender.EnqueueResult(SuspendResult.Completed);
        harness.Processor.OnJobProcessorError += (_, _) => throw new InvalidOperationException("subscriber failed");

        // Act
        harness.Processor.Start();
        (object? sender, Exception reported) = await harness.NextErrorAsync(cancellationToken);
        await harness.Suspender.NextSuspensionAsync(cancellationToken);
        await harness.Factory.NextCallAsync(cancellationToken);
        await harness.Factory.NextCallAsync(cancellationToken);

        // Assert
        Assert.Same(harness.Processor, sender);
        Assert.Same(failure, reported);
        Assert.Equal(1, harness.ErrorCount);
    }

    [Fact(Timeout = 10_000)]
    public async Task ProcessLoop_WhenSuspensionFails_ReportsErrorAndEnds()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var failure = new InvalidOperationException("suspension failed");
        harness.Factory.EnqueueJob(null);
        harness.Suspender.Enqueue((_, _) => Task.FromException<SuspendResult>(failure));

        // Act
        harness.Processor.Start();
        (object? sender, Exception reported) = await harness.NextErrorAsync(cancellationToken);
        await harness.Processor.StopAsync(cancellationToken);

        // Assert
        Assert.Same(harness.Processor, sender);
        Assert.Same(failure, reported);
        Assert.Equal(1, harness.ErrorCount);
        Assert.Equal(1, harness.Factory.CallCount);
    }

    [Fact(Timeout = 10_000)]
    public async Task ProcessLoop_WhenIntervalReadFails_ReportsErrorAndEnds()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var failure = new InvalidOperationException("interval read failed");
        harness.Factory.EnqueueJob(null);
        harness.Interval.OnCurrent = () => throw failure;

        // Act
        harness.Processor.Start();
        (object? sender, Exception reported) = await harness.NextErrorAsync(cancellationToken);
        await harness.Processor.StopAsync(cancellationToken);

        // Assert
        Assert.Same(harness.Processor, sender);
        Assert.Same(failure, reported);
        Assert.Equal(1, harness.ErrorCount);
    }

    [Fact(Timeout = 10_000)]
    public async Task ProcessLoop_WhenIntervalIncreaseFails_ReportsErrorAndEnds()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var failure = new InvalidOperationException("interval increase failed");
        harness.Factory.EnqueueJob(null);
        harness.Suspender.EnqueueResult(SuspendResult.Completed);
        harness.Interval.OnIncrease = () => throw failure;

        // Act
        harness.Processor.Start();
        (object? sender, Exception reported) = await harness.NextErrorAsync(cancellationToken);
        await harness.Processor.StopAsync(cancellationToken);

        // Assert
        Assert.Same(harness.Processor, sender);
        Assert.Same(failure, reported);
        Assert.Equal(1, harness.ErrorCount);
    }

    [Fact(Timeout = 10_000)]
    public async Task ProcessLoop_WhenIntervalResetFails_ReportsErrorAndEnds()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var failure = new InvalidOperationException("interval reset failed");
        harness.Factory.EnqueueJob(new SignalingJob());
        harness.Interval.OnReset = () => throw failure;

        // Act
        harness.Processor.Start();
        (object? sender, Exception reported) = await harness.NextErrorAsync(cancellationToken);
        await harness.Processor.StopAsync(cancellationToken);

        // Assert
        Assert.Same(harness.Processor, sender);
        Assert.Same(failure, reported);
        Assert.Equal(1, harness.ErrorCount);
    }

    [Fact(Timeout = 10_000)]
    public async Task Ping_AfterProcessingLoopEnds_Throws()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        harness.Factory.EnqueueJob(null);
        harness.Suspender.Enqueue((_, _) => Task.FromException<SuspendResult>(
            new InvalidOperationException("suspension failed")));
        harness.Processor.Start();
        await harness.NextErrorAsync(cancellationToken);

        // Act
        InvalidOperationException exception;
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                harness.Processor.Ping();
            }
            catch (InvalidOperationException result)
            {
                exception = result;
                break;
            }
            await Task.Yield();
        }

        // Assert
        Assert.Equal("Processing is not running.", exception.Message);
    }

    [Fact(Timeout = 10_000)]
    public async Task StopAsync_WhileWaitingForCapacity_CancelsProcessingWithoutError()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        harness.Runner.WaitForCapacity = token => Task.Delay(Timeout.Infinite, token);
        harness.Processor.Start();
        CancellationToken processingToken = await harness.Runner.NextCapacityWaitAsync(cancellationToken);

        // Act
        await harness.Processor.StopAsync(cancellationToken);

        // Assert
        Assert.True(processingToken.IsCancellationRequested);
        Assert.Equal(0, harness.Factory.CallCount);
        Assert.Equal(0, harness.ErrorCount);
        Assert.Equal(cancellationToken, await harness.Runner.NextStopAsync(cancellationToken));
    }

    [Fact(Timeout = 10_000)]
    public async Task StopAsync_WhileCreatingJob_CancelsProcessingWithoutError()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        harness.Processor.Start();
        CancellationToken processingToken = await harness.Factory.NextCallAsync(cancellationToken);

        // Act
        await harness.Processor.StopAsync(cancellationToken);

        // Assert
        Assert.True(processingToken.IsCancellationRequested);
        Assert.Equal(0, harness.ErrorCount);
        Assert.Equal(cancellationToken, await harness.Runner.NextStopAsync(cancellationToken));
    }

    [Fact(Timeout = 10_000)]
    public async Task StopAsync_WhileSuspended_CancelsProcessingWithoutError()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        harness.Factory.EnqueueJob(null);
        harness.Processor.Start();
        await harness.Suspender.NextSuspensionAsync(cancellationToken);

        // Act
        await harness.Processor.StopAsync(cancellationToken);

        // Assert
        Assert.Equal(0, harness.ErrorCount);
        Assert.Equal(cancellationToken, await harness.Runner.NextStopAsync(cancellationToken));
    }

    [Fact(Timeout = 10_000)]
    public async Task StopAsync_WhenIterationIgnoresCancellation_WaitsBeforeStoppingRunner()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        TaskCompletionSource<bool> release = NewSignal();
        harness.Factory.Enqueue(async token =>
        {
            await release.Task;
            token.ThrowIfCancellationRequested();
            return null;
        });
        harness.Processor.Start();
        CancellationToken processingToken = await harness.Factory.NextCallAsync(cancellationToken);

        try
        {
            // Act
            Task stop = harness.Processor.StopAsync(cancellationToken);

            // Assert
            await WaitForCancellationAsync(processingToken, cancellationToken);
            Assert.False(stop.IsCompleted);
            Assert.Equal(0, harness.Runner.StopCount);

            release.SetResult(true);
            await stop.WaitAsync(cancellationToken);
            Assert.Equal(1, harness.Runner.StopCount);
        }
        finally
        {
            release.TrySetResult(true);
        }
    }

    [Fact(Timeout = 10_000)]
    public async Task StopAsync_WhenCalledTwice_BothCallsWaitForProcessing()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        TaskCompletionSource<bool> release = NewSignal();
        harness.Factory.Enqueue(async token =>
        {
            await release.Task;
            token.ThrowIfCancellationRequested();
            return null;
        });
        harness.Processor.Start();
        CancellationToken processingToken = await harness.Factory.NextCallAsync(cancellationToken);

        try
        {
            // Act
            Task firstStop = harness.Processor.StopAsync(cancellationToken);
            Task secondStop = harness.Processor.StopAsync(cancellationToken);
            await WaitForCancellationAsync(processingToken, cancellationToken);

            // Assert
            Assert.False(firstStop.IsCompleted);
            Assert.False(secondStop.IsCompleted);
            release.SetResult(true);
            await firstStop.WaitAsync(cancellationToken);
            await secondStop.WaitAsync(cancellationToken);
            Assert.Equal(2, harness.Runner.StopCount);
        }
        finally
        {
            release.TrySetResult(true);
        }
    }

    [Fact(Timeout = 10_000)]
    public async Task StopAsync_WhenCallerCancelsWait_ProcessingStillStops()
    {
        // Arrange
        await using var harness = new ProcessorHarness();
        using var waitCancellation = new CancellationTokenSource();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        TaskCompletionSource<bool> release = NewSignal();
        harness.Factory.Enqueue(async token =>
        {
            await release.Task;
            token.ThrowIfCancellationRequested();
            return null;
        });
        harness.Processor.Start();
        CancellationToken processingToken = await harness.Factory.NextCallAsync(cancellationToken);

        try
        {
            Task firstStop = harness.Processor.StopAsync(waitCancellation.Token);
            await WaitForCancellationAsync(processingToken, cancellationToken);

            // Act
            waitCancellation.Cancel();

            // Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => firstStop);
            Assert.Equal(waitCancellation.Token, await harness.Runner.NextStopAsync(cancellationToken));
            Task continuedStop = harness.Processor.StopAsync(cancellationToken);
            Assert.False(continuedStop.IsCompleted);
            release.SetResult(true);
            await continuedStop.WaitAsync(cancellationToken);
            Assert.Equal(0, harness.ErrorCount);
        }
        finally
        {
            release.TrySetResult(true);
        }
    }

    [Fact(Timeout = 10_000)]
    public async Task SettingsConstructor_CreatesAndExecutesJob()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var factory = new ScriptedJobFactory();
        var job = new SignalingJob();
        factory.EnqueueJob(job);
        var settings = new JobProcessorSettings
        {
            ConcurrencyLimit = 1,
            JobProcessingTimeout = Timeout.InfiniteTimeSpan,
            InitialInterval = TimeSpan.FromSeconds(1),
            MaxInterval = TimeSpan.FromSeconds(2),
            IntervalMultiplier = 2
        };
        using var processor = new JobProcessor(factory, settings);

        try
        {
            // Act
            processor.Start();
            await job.Executed.WaitAsync(cancellationToken);
            await factory.NextCallAsync(cancellationToken);
            await factory.NextCallAsync(cancellationToken);
        }
        finally
        {
            await processor.StopAsync(cancellationToken);
        }
    }

    private static TaskCompletionSource<bool> NewSignal() =>
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private static async Task WaitForCancellationAsync(CancellationToken observedToken, CancellationToken testToken)
    {
        TaskCompletionSource<bool> canceled = NewSignal();
        using CancellationTokenRegistration registration = observedToken.Register(() => canceled.TrySetResult(true));
        await canceled.Task.WaitAsync(testToken);
    }

    private sealed class ProcessorHarness : IDisposable, IAsyncDisposable
    {
        private readonly ConcurrentQueue<(object? Sender, Exception Error)> _errors = new();
        private readonly Channel<(object? Sender, Exception Error)> _errorEvents = Channel.CreateUnbounded<(object?, Exception)>();

        public ProcessorHarness()
        {
            Processor = new JobProcessor(Factory, Runner, Suspender, Interval);
            Processor.OnJobProcessorError += (sender, error) =>
            {
                _errors.Enqueue((sender, error));
                _errorEvents.Writer.TryWrite((sender, error));
            };
        }

        public ScriptedJobFactory Factory { get; } = new();

        public RecordingJobRunner Runner { get; } = new();

        public RecordingSuspender Suspender { get; } = new();

        public RecordingIntervalCalculator Interval { get; } = new();

        public JobProcessor Processor { get; }

        public int ErrorCount => _errors.Count;

        public ValueTask<(object? Sender, Exception Error)> NextErrorAsync(CancellationToken cancellationToken) =>
            _errorEvents.Reader.ReadAsync(cancellationToken);

        public void Dispose() => Processor.Dispose();

        public async ValueTask DisposeAsync()
        {
            try
            {
                await Processor.StopAsync(TestContext.Current.CancellationToken);
            }
            finally
            {
                Processor.Dispose();
            }
        }
    }

    private sealed class ScriptedJobFactory : IJobFactory
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task<IJob?>>> _steps = new();
        private readonly Channel<CancellationToken> _calls = Channel.CreateUnbounded<CancellationToken>();
        private int _callCount;

        public int CallCount => Volatile.Read(ref _callCount);

        public void EnqueueJob(IJob? job) => Enqueue(_ => Task.FromResult(job));

        public void Enqueue(Func<CancellationToken, Task<IJob?>> step) => _steps.Enqueue(step);

        public ValueTask<CancellationToken> NextCallAsync(CancellationToken cancellationToken) =>
            _calls.Reader.ReadAsync(cancellationToken);

        public Task<IJob?> TryCreateJobAsync(CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _callCount);
            _calls.Writer.TryWrite(cancellationToken);
            return _steps.TryDequeue(out Func<CancellationToken, Task<IJob?>>? step)
                ? step(cancellationToken)
                : WaitUntilCanceledAsync(cancellationToken);
        }

        private static async Task<IJob?> WaitUntilCanceledAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
            return null;
        }
    }

    private sealed class RecordingJobRunner : IJobRunner
    {
        private readonly Channel<CancellationToken> _capacityWaits = Channel.CreateUnbounded<CancellationToken>();
        private readonly Channel<IJob> _runs = Channel.CreateUnbounded<IJob>();
        private readonly Channel<CancellationToken> _stops = Channel.CreateUnbounded<CancellationToken>();
        private int _runCount;
        private int _stopCount;

        public Func<CancellationToken, Task> WaitForCapacity { get; set; } = _ => Task.CompletedTask;

        public Action<IJob>? OnRun { get; set; }

        public int RunCount => Volatile.Read(ref _runCount);

        public int StopCount => Volatile.Read(ref _stopCount);

        public bool IsDisposed { get; private set; }

        public ValueTask<CancellationToken> NextCapacityWaitAsync(CancellationToken cancellationToken) =>
            _capacityWaits.Reader.ReadAsync(cancellationToken);

        public ValueTask<IJob> NextRunAsync(CancellationToken cancellationToken) =>
            _runs.Reader.ReadAsync(cancellationToken);

        public ValueTask<CancellationToken> NextStopAsync(CancellationToken cancellationToken) =>
            _stops.Reader.ReadAsync(cancellationToken);

        public Task WaitForCapacityAsync(CancellationToken cancellationToken)
        {
            _capacityWaits.Writer.TryWrite(cancellationToken);
            return WaitForCapacity(cancellationToken);
        }

        public void Run(IJob job)
        {
            Interlocked.Increment(ref _runCount);
            _runs.Writer.TryWrite(job);
            OnRun?.Invoke(job);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _stopCount);
            _stops.Writer.TryWrite(cancellationToken);
            return Task.CompletedTask;
        }

        public void Dispose() => IsDisposed = true;
    }

    private sealed class RecordingSuspender : IProcessingSuspender
    {
        private readonly ConcurrentQueue<Func<TimeSpan, CancellationToken, Task<SuspendResult>>> _steps = new();
        private readonly Channel<TimeSpan> _suspensions = Channel.CreateUnbounded<TimeSpan>();
        private readonly Channel<bool> _resumes = Channel.CreateUnbounded<bool>();
        private int _suspendCount;

        public int SuspendCount => Volatile.Read(ref _suspendCount);

        public void EnqueueResult(SuspendResult result) =>
            Enqueue((_, _) => Task.FromResult(result));

        public void Enqueue(Func<TimeSpan, CancellationToken, Task<SuspendResult>> step) => _steps.Enqueue(step);

        public ValueTask<TimeSpan> NextSuspensionAsync(CancellationToken cancellationToken) =>
            _suspensions.Reader.ReadAsync(cancellationToken);

        public ValueTask<bool> NextResumeAsync(CancellationToken cancellationToken) =>
            _resumes.Reader.ReadAsync(cancellationToken);

        public Task<SuspendResult> SuspendAsync(TimeSpan duration, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _suspendCount);
            _suspensions.Writer.TryWrite(duration);
            return _steps.TryDequeue(out Func<TimeSpan, CancellationToken, Task<SuspendResult>>? step)
                ? step(duration, cancellationToken)
                : WaitUntilCanceledAsync(cancellationToken);
        }

        public void TryResume() => _resumes.Writer.TryWrite(true);

        private static async Task<SuspendResult> WaitUntilCanceledAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
            return SuspendResult.Completed;
        }
    }

    private sealed class RecordingIntervalCalculator : IIntervalCalculator
    {
        private readonly Channel<string> _changes = Channel.CreateUnbounded<string>();
        private TimeSpan _current = TimeSpan.FromTicks(10);
        private int _increaseCount;

        public Func<TimeSpan>? OnCurrent { get; set; }

        public Action? OnIncrease { get; set; }

        public Action? OnReset { get; set; }

        public TimeSpan Current => OnCurrent?.Invoke() ?? _current;

        public int IncreaseCount => Volatile.Read(ref _increaseCount);

        public ValueTask<string> NextChangeAsync(CancellationToken cancellationToken) =>
            _changes.Reader.ReadAsync(cancellationToken);

        public void Increase()
        {
            OnIncrease?.Invoke();
            _current = TimeSpan.FromTicks(_current.Ticks * 2);
            Interlocked.Increment(ref _increaseCount);
            _changes.Writer.TryWrite("Increase");
        }

        public void Reset()
        {
            OnReset?.Invoke();
            _current = TimeSpan.FromTicks(10);
            _changes.Writer.TryWrite("Reset");
        }
    }

    private sealed class SignalingJob : IJob
    {
        private readonly TaskCompletionSource<bool> _executed = NewSignal();

        public Task Executed => _executed.Task;

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _executed.TrySetResult(true);
            return Task.CompletedTask;
        }
    }
}
