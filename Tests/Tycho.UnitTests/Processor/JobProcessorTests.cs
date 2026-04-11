using Moq;
using Tycho.Processor;

namespace Tycho.UnitTests.Processor;

public class JobProcessorTests
{
    private static readonly TimeSpan _waitingTimeout = TimeSpan.FromSeconds(5);

    private static readonly TimeSpan _initialInterval = TimeSpan.FromMilliseconds(10);
    private static readonly TimeSpan _maxInterval = TimeSpan.FromMilliseconds(50);
    private static readonly double _intervalMultiplier = 2;

    private readonly Mock<IJobFactory> _factoryMock = new();

    #region ACTIVATION

    [Fact]
    public async Task JobProcessor_OnActivation_StartsProcessingSchedule()
    {
        // Arrange
        var factoryCalledSignal = new ManualResetEventSlim(false);

        _factoryMock.Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync([])
                    .Callback(factoryCalledSignal.Set);

        using var sut = CreateSut();

        // Act
        sut.Activate();

        // Assert
        Assert.True(factoryCalledSignal.Wait(_waitingTimeout, CancellationToken.None));
    }

    #endregion ACTIVATION

    #region PROCESSING CAPACITY

    [Fact]
    public async Task JobProcessor_WithFullCapacity_CreatesNumberOfJobsEqualToConcurrencyLimit()
    {
        // Arrange
        var concurrencyLimit = 5;
        int? capturedMaxCount = null;
        var factoryCalledSignal = new ManualResetEventSlim(false);

        _factoryMock.Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync([])
                    .Callback<int, CancellationToken>((maxCount, _) =>
                    {
                        capturedMaxCount ??= maxCount;
                        factoryCalledSignal.Set();
                    });

        using var sut = CreateSut(s => s.ConcurrencyLimit = concurrencyLimit);

        // Act
        sut.Activate();

        // Assert
        Assert.True(factoryCalledSignal.Wait(_waitingTimeout, CancellationToken.None));
        Assert.Equal(concurrencyLimit, capturedMaxCount);
    }

    [Fact]
    public async Task JobProcessor_WithSomeCapacity_CreatesNumberOfJobsEqualToRemainingCapacity()
    {
        // Arrange
        var concurrencyLimit = 5;
        var capturedMaxCounts = new List<int>();
        var factoryCalledSignal = new CountdownEvent(3);

        _factoryMock.Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync([CreateInfiniteJob().Object, CreateInfiniteJob().Object])
                    .Callback<int, CancellationToken>((maxCount, _) =>
                    {
                        capturedMaxCounts.Add(maxCount);
                        factoryCalledSignal.Signal();
                    });

        using var sut = CreateSut(s => s.ConcurrencyLimit = concurrencyLimit);

        // Act
        sut.Activate();

        // Assert
        Assert.True(factoryCalledSignal.Wait(_waitingTimeout, CancellationToken.None));
        Assert.Equal([5, 3, 1], capturedMaxCounts);
    }

    [Fact]
    public async Task JobProcessor_WithNoCapacity_DoesNotCreateAnyJobsOverCapacity()
    {
        // Arrange
        var concurrencyLimit = 1;

        _factoryMock.Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync([CreateInfiniteJob().Object]);

        using var sut = CreateSut(s => s.ConcurrencyLimit = concurrencyLimit);

        // Act
        sut.Activate();
        await WaitForPotentialNextIteration();

        // Assert
        _factoryMock.Verify(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task JobProcessor_WithZeroCapacity_DoesNotCreateAnyJobs()
    {
        // Arrange
        var concurrencyLimit = 0;

        _factoryMock.Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync([]);

        using var sut = CreateSut(s => s.ConcurrencyLimit = concurrencyLimit);

        // Act
        sut.Activate();
        await WaitTillProcessorIdles();

        // Assert
        _factoryMock.Verify(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion PROCESSING CAPACITY

    #region JOB EXECUTION



    #endregion JOB EXECUTION

    #region PROCESSOR TIMING



    #endregion PROCESSOR TIMING

    #region ERROR HANDLING



    #endregion ERROR HANDLING

    #region DISPOSAL



    #endregion DISPOSAL

    // --- Job Execution ---

    [Fact]
    public async Task ProcessSchedule_WithJobsReturned_ExecutesAllJobs()
    {
        // Arrange
        var jobExecutedSignal = new SemaphoreSlim(0, 10);

        var firstJobMock = new Mock<IJob>();
        firstJobMock
            .Setup(j => j.ExecuteAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(() => jobExecutedSignal.Release());

        var secondJobMock = new Mock<IJob>();
        secondJobMock
            .Setup(j => j.ExecuteAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(() => jobExecutedSignal.Release());

        int callCount = 0;
        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                if (Interlocked.Increment(ref callCount) == 1) return [firstJobMock.Object, secondJobMock.Object];
                return [];
            });

        using var sut = CreateSut();

        // Act
        sut.Activate();
        await jobExecutedSignal.WaitAsync(_waitingTimeout);
        await jobExecutedSignal.WaitAsync(_waitingTimeout);

        // Assert
        firstJobMock.Verify(j => j.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);
        secondJobMock.Verify(j => j.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // --- Concurrency Limiting ---

    [Fact]
    public async Task ProcessSchedule_WhenJobCompletes_RestoresCapacity()
    {
        // Arrange
        var jobStartedSignal = new SemaphoreSlim(0, 1);
        var jobBlocker = new TaskCompletionSource();
        var blockingJobMock = CreateBlockingJob(jobStartedSignal, jobBlocker);

        int factoryCallCount = 0;
        var factoryCalledAfterRelease = new SemaphoreSlim(0, 100);

        _ = _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var n = Interlocked.Increment(ref factoryCallCount);
                if (n == 1) return [blockingJobMock.Object];
                factoryCalledAfterRelease.Release();
                return [];
            });

        using var sut = CreateSut(s => s.ConcurrencyLimit = 1);

        // Act
        sut.Activate();
        await jobStartedSignal.WaitAsync(_waitingTimeout);
        await Task.Delay(200);
        var callCountWhileBlocked = Volatile.Read(ref factoryCallCount);

        jobBlocker.SetResult();
        sut.Activate();
        await factoryCalledAfterRelease.WaitAsync(_waitingTimeout);

        // Assert
        Assert.True(Volatile.Read(ref factoryCallCount) > callCountWhileBlocked);
    }

    // --- Backoff ---

    [Fact]
    public async Task ProcessSchedule_WithNoJobsAvailable_EventuallyStopsPolling()
    {
        // Arrange
        int factoryCallCount = 0;

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<IJob>())
            .Callback(() => Interlocked.Increment(ref factoryCallCount));

        using var sut = CreateSut(s =>
        {
            s.InitialInterval = TimeSpan.FromMilliseconds(10);
            s.IntervalMultiplier = 2;
            s.MaxInterval = TimeSpan.FromMilliseconds(50);
        });

        // Act
        sut.Activate();
        await Task.Delay(500);
        var callCountSnapshot = Volatile.Read(ref factoryCallCount);
        await Task.Delay(500);

        // Assert
        Assert.True(callCountSnapshot >= 3);
        Assert.Equal(callCountSnapshot, Volatile.Read(ref factoryCallCount));
    }

    // --- Reentrancy Guard ---

    [Fact]
    public async Task ProcessSchedule_WhenAlreadyProcessing_SkipsOverlappingTick()
    {
        // Arrange
        var factoryBlocker = new TaskCompletionSource<IReadOnlyCollection<IJob>>();
        int factoryCallCount = 0;

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns<int, CancellationToken>((_, __) =>
            {
                Interlocked.Increment(ref factoryCallCount);
                return factoryBlocker.Task;
            });

        using var sut = CreateSut(s =>
        {
            s.InitialInterval = TimeSpan.FromMilliseconds(10);
            s.ScheduleProcessingTimeout = TimeSpan.FromSeconds(30);
        });

        // Act
        sut.Activate();
        await Task.Delay(200);
        var callCountWhileBlocked = Volatile.Read(ref factoryCallCount);
        factoryBlocker.SetResult(Array.Empty<IJob>());

        // Assert
        Assert.Equal(1, callCountWhileBlocked);
    }

    // --- Schedule Processing Errors ---

    [Fact]
    public async Task ProcessSchedule_WhenFactoryThrows_RaisesOnScheduleProcessingError()
    {
        // Arrange
        var expectedException = new InvalidOperationException("factory error");
        Exception? capturedError = null;
        var errorSignal = new SemaphoreSlim(0, 1);

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        using var sut = CreateSut();
        sut.OnScheduleProcessingError += (_, ex) =>
        {
            capturedError = ex;
            errorSignal.Release();
        };

        // Act
        sut.Activate();
        await errorSignal.WaitAsync(_waitingTimeout);

        // Assert
        Assert.Same(expectedException, capturedError);
    }

    [Fact]
    public async Task ProcessSchedule_WhenFactoryThrows_ContinuesProcessingNextTick()
    {
        // Arrange
        int callCount = 0;
        var secondCallSignal = new SemaphoreSlim(0, 1);

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns<int, CancellationToken>((_, __) =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    return Task.FromException<IReadOnlyCollection<IJob>>(new InvalidOperationException("factory error"));
                secondCallSignal.Release();
                return Task.FromResult<IReadOnlyCollection<IJob>>(Array.Empty<IJob>());
            });

        using var sut = CreateSut();

        // Act
        sut.Activate();
        await secondCallSignal.WaitAsync(_waitingTimeout);

        // Assert
        Assert.True(Volatile.Read(ref callCount) >= 2);
    }

    [Fact]
    public async Task ProcessSchedule_WhenScheduleErrorSubscriberThrows_DoesNotCrash()
    {
        // Arrange
        int callCount = 0;
        var secondCallSignal = new SemaphoreSlim(0, 1);

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns<int, CancellationToken>((_, __) =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    return Task.FromException<IReadOnlyCollection<IJob>>(new InvalidOperationException("factory error"));
                secondCallSignal.Release();
                return Task.FromResult<IReadOnlyCollection<IJob>>(Array.Empty<IJob>());
            });

        using var sut = CreateSut();
        sut.OnScheduleProcessingError += (_, _) => throw new Exception("subscriber error");

        // Act
        sut.Activate();
        await secondCallSignal.WaitAsync(_waitingTimeout);

        // Assert
        Assert.True(Volatile.Read(ref callCount) >= 2);
    }

    // --- Job Processing Errors ---

    [Fact]
    public async Task ProcessJob_WhenJobThrows_RaisesOnJobProcessingError()
    {
        // Arrange
        var expectedException = new InvalidOperationException("job error");
        Exception? capturedError = null;
        var errorSignal = new SemaphoreSlim(0, 1);

        var jobMock = new Mock<IJob>();
        jobMock
            .Setup(j => j.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        int callCount = 0;

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    return new IJob[] { jobMock.Object };
                return Array.Empty<IJob>();
            });

        using var sut = CreateSut();
        sut.OnJobProcessingError += (_, ex) =>
        {
            capturedError = ex;
            errorSignal.Release();
        };

        // Act
        sut.Activate();
        await errorSignal.WaitAsync(_waitingTimeout);

        // Assert
        Assert.Same(expectedException, capturedError);
    }

    [Fact]
    public async Task ProcessJob_WhenJobThrows_RestoresCapacity()
    {
        // Arrange
        var concurrencyLimit = 3;
        var errorSignal = new SemaphoreSlim(0, 1);
        var capacitySignal = new SemaphoreSlim(0, 10);
        int? capturedCapacityAfterError = null;

        var jobMock = new Mock<IJob>();
        jobMock
            .Setup(j => j.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("job error"));

        var errorFired = false;
        int callCount = 0;

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int maxCount, CancellationToken _) =>
            {
                var n = Interlocked.Increment(ref callCount);
                if (n == 1)
                    return new IJob[] { jobMock.Object };
                if (Volatile.Read(ref errorFired))
                {
                    capturedCapacityAfterError ??= maxCount;
                    capacitySignal.Release();
                }
                return Array.Empty<IJob>();
            });

        using var sut = CreateSut(s => s.ConcurrencyLimit = concurrencyLimit);
        sut.OnJobProcessingError += (_, _) =>
        {
            Volatile.Write(ref errorFired, true);
            errorSignal.Release();
        };

        // Act
        sut.Activate();
        await errorSignal.WaitAsync(_waitingTimeout);
        sut.Activate();
        await capacitySignal.WaitAsync(_waitingTimeout);

        // Assert
        Assert.Equal(concurrencyLimit, capturedCapacityAfterError);
    }

    [Fact]
    public async Task ProcessJob_WhenJobErrorSubscriberThrows_DoesNotCrash()
    {
        // Arrange
        var jobMock = new Mock<IJob>();
        jobMock
            .Setup(j => j.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("job error"));

        int callCount = 0;
        var secondFactoryCallSignal = new SemaphoreSlim(0, 1);

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                if (Interlocked.Increment(ref callCount) == 1)
                    return [jobMock.Object];
                secondFactoryCallSignal.Release();
                return [];
            });

        using var sut = CreateSut();
        sut.OnJobProcessingError += (_, _) => throw new Exception("subscriber error");

        // Act
        sut.Activate();
        await secondFactoryCallSignal.WaitAsync(_waitingTimeout);

        // Assert
        Assert.True(Volatile.Read(ref callCount) >= 2);
    }

    // --- Disposal ---

    [Fact]
    public async Task Dispose_StopsProcessing()
    {
        // Arrange
        int factoryCallCount = 0;
        var factoryCalledSignal = new SemaphoreSlim(0, 100);

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([])
            .Callback(() =>
            {
                Interlocked.Increment(ref factoryCallCount);
                factoryCalledSignal.Release();
            });

        var sut = CreateSut();

        // Act
        sut.Activate();
        await factoryCalledSignal.WaitAsync(_waitingTimeout);
        sut.Dispose();
        var callCountAfterDispose = Volatile.Read(ref factoryCallCount);
        await Task.Delay(200);

        // Assert
        Assert.Equal(callCountAfterDispose, Volatile.Read(ref factoryCallCount));
    }

    [Fact]
    public void Dispose_CompletesSuccessfully()
    {
        // Arrange
        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var sut = CreateSut();

        // Act & Assert
        sut.Dispose();
    }

    private JobProcessor CreateSut(Action<JobProcessorSettings>? configure = null)
    {
        var settings = new JobProcessorSettings
        {
            ConcurrencyLimit = 100,
            InitialInterval = _initialInterval,
            IntervalMultiplier = _intervalMultiplier,
            MaxInterval = _maxInterval,
            ScheduleProcessingTimeout = TimeSpan.FromSeconds(5),
            JobProcessingTimeout = TimeSpan.FromSeconds(5),
        };
        configure?.Invoke(settings);
        return new JobProcessor(_factoryMock.Object, settings);
    }

    private static Mock<IJob> CreateInfiniteJob()
    {
        var jobMock = new Mock<IJob>();
        jobMock.Setup(j => j.ExecuteAsync(It.IsAny<CancellationToken>()))
               .Returns(async (CancellationToken cancellationToken) =>
               {
                   await Task.Delay(Timeout.Infinite, cancellationToken);
               });
        return jobMock;
    }

    private static Mock<IJob> CreateBlockingJob(SemaphoreSlim startedSignal, TaskCompletionSource blocker)
    {
        var jobMock = new Mock<IJob>();
        jobMock
            .Setup(j => j.ExecuteAsync(It.IsAny<CancellationToken>()))
            .Returns(async (CancellationToken _) =>
            {
                startedSignal.Release();
                await blocker.Task;
            });
        return jobMock;
    }

    private async static Task WaitForPotentialNextIteration()
    {
        // an arbitrary delay long enough to allow for the processor
        // to execute another tick if it was going to
        var timeToWait = _maxInterval * 2;
        await Task.Delay(timeToWait);
    }

    private async static Task WaitTillProcessorIdles()
    {
        var timeToWait = _initialInterval;

        var currentInterval = _initialInterval;
        while (currentInterval * _intervalMultiplier <= _maxInterval)
        {
            currentInterval *= _intervalMultiplier;
            timeToWait += currentInterval;
        }

        var safetyMargin = _maxInterval;
        await Task.Delay(timeToWait + safetyMargin);
    }
}
