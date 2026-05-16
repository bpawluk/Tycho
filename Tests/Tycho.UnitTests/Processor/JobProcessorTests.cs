using System.Security.Cryptography;
using System.Text;
using Moq;
using Tycho.Processor;

namespace Tycho.UnitTests.Processor;

public class JobProcessorTests
{
    private static readonly TimeSpan s_waitingTimeout = TimeSpan.FromSeconds(10);

    private static readonly TimeSpan s_initialInterval = TimeSpan.FromMilliseconds(5);
    private static readonly TimeSpan s_maxInterval = TimeSpan.FromMilliseconds(50);
    private static readonly double s_intervalMultiplier = 2;

    private readonly Mock<IJobFactory> _factoryMock = new();

    #region ACTIVATION

    [Fact]
    public async Task JobProcessor_OnActivation_StartsProcessing()
    {
        // Arrange
        var factoryCalledSignal = new ManualResetEventSlim(false);

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([])
            .Callback(factoryCalledSignal.Set);

        using JobProcessor sut = CreateSut();

        // Act
        sut.Activate();

        // Assert
        Assert.True(factoryCalledSignal.Wait(s_waitingTimeout, CancellationToken.None));
    }

    #endregion ACTIVATION

    #region PROCESSING CAPACITY

    [Fact]
    public async Task JobProcessor_WithFullCapacity_CreatesNumberOfJobsEqualToConcurrencyLimit()
    {
        // Arrange
        int concurrencyLimit = 5;
        int? capturedMaxCount = null;
        var factoryCalledSignal = new ManualResetEventSlim(false);

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([])
            .Callback<int, CancellationToken>((maxCount, _) =>
            {
                capturedMaxCount ??= maxCount;
                factoryCalledSignal.Set();
            });

        using JobProcessor sut = CreateSut(s => s.ConcurrencyLimit = concurrencyLimit);

        // Act
        sut.Activate();

        // Assert
        Assert.True(factoryCalledSignal.Wait(s_waitingTimeout, CancellationToken.None));
        Assert.Equal(concurrencyLimit, capturedMaxCount);
    }

    [Fact]
    public async Task JobProcessor_WithSomeCapacity_CreatesNumberOfJobsEqualToRemainingCapacity()
    {
        // Arrange
        int concurrencyLimit = 5;
        var capturedMaxCounts = new List<int>();
        var factoryCalledSignal = new CountdownEvent(3);

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateInfiniteJob().Object, CreateInfiniteJob().Object])
            .Callback<int, CancellationToken>((maxCount, _) =>
            {
                capturedMaxCounts.Add(maxCount);
                factoryCalledSignal.Signal();
            });

        using JobProcessor sut = CreateSut(s => s.ConcurrencyLimit = concurrencyLimit);

        // Act
        sut.Activate();

        // Assert
        Assert.True(factoryCalledSignal.Wait(s_waitingTimeout, CancellationToken.None));
        Assert.Equal([5, 3, 1], capturedMaxCounts);
    }

    [Fact]
    public async Task JobProcessor_WithNoCapacity_DoesNotCreateAnyJobsOverCapacity()
    {
        // Arrange
        int concurrencyLimit = 1;

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateInfiniteJob().Object]);

        using JobProcessor sut = CreateSut(s => s.ConcurrencyLimit = concurrencyLimit);

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
        int concurrencyLimit = 0;

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        using JobProcessor sut = CreateSut(s => s.ConcurrencyLimit = concurrencyLimit);

        // Act
        sut.Activate();
        await WaitTillProcessorIdles();

        // Assert
        _factoryMock.Verify(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion PROCESSING CAPACITY

    #region JOB EXECUTION

    [Fact]
    public async Task JobProcessor_WithJobsToProcess_ExecutesAllJobsAndEventuallyStopsPolling()
    {
        // Arrange
        int concurrencyLimit = 3;
        int jobsToExecuteCount = 10;

        (Mock<IJob> Job, ManualResetEventSlim Signal)[] jobsToExecute = [.. Enumerable.Range(0, jobsToExecuteCount).Select(_ =>
        {
            var jobCompletedSignal = new ManualResetEventSlim(false);
            Mock<IJob> jobMock = CreateActualJob(jobCompletedSignal);
            return (Job: jobMock, Signal: jobCompletedSignal);
        })];

        int factoryCallCount = 0;
        var jobsQueue = new Queue<Mock<IJob>>(jobsToExecute.Select(jobData => jobData.Job));

        _factoryMock
           .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync((int maxCount, CancellationToken _) =>
           {
               Interlocked.Increment(ref factoryCallCount);
               var jobs = new List<IJob>();
               for (int i = 0; i < maxCount; i++)
               {
                   if (jobsQueue.TryDequeue(out Mock<IJob>? nextJob))
                   {
                       jobs.Add(nextJob.Object);
                   }
                   else
                   {
                       break;
                   }
               }
               return jobs;
           });

        using JobProcessor sut = CreateSut(s => s.ConcurrencyLimit = concurrencyLimit);

        // STAGE 1

        // Act
        sut.Activate();

        // Assert
        foreach ((Mock<IJob> _, ManualResetEventSlim? Signal) in jobsToExecute)
        {
            Assert.True(Signal.Wait(s_waitingTimeout, CancellationToken.None));
        }

        // STAGE 2

        // Act
        await WaitTillProcessorIdles();
        int finalFactoryCallCount = Volatile.Read(ref factoryCallCount);
        await WaitForPotentialNextIteration();

        // Assert

        Assert.Equal(finalFactoryCallCount, Volatile.Read(ref factoryCallCount));
    }

    [Fact]
    public async Task JobProcessor_WhenJobCompletes_RestoresCapacity()
    {
        // Arrange
        int concurrencyLimit = 3;

        var jobMock = new Mock<IJob>();
        var completeJobSignal = new ManualResetEventSlim(false);
        bool jobCompleted = false;

        jobMock
            .Setup(j => j.ExecuteAsync(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                completeJobSignal.Wait(s_waitingTimeout, CancellationToken.None);
                Volatile.Write(ref jobCompleted, true);
                return Task.CompletedTask;
            });

        int factoryCallCount = 0;
        int capturedCapacity = -1;

        var factoryCalledBeforeCompletionSignal = new ManualResetEventSlim(false);
        var factoryCalledAfterCompletionSignal = new ManualResetEventSlim(false);

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int maxCount, CancellationToken _) =>
            {
                Volatile.Write(ref capturedCapacity, maxCount);

                if (Interlocked.Increment(ref factoryCallCount) == 1)
                {
                    return [jobMock.Object];
                }

                if (Volatile.Read(ref jobCompleted))
                {
                    factoryCalledAfterCompletionSignal.Set();
                }
                else
                {
                    factoryCalledBeforeCompletionSignal.Set();
                }

                return [];
            });

        using JobProcessor sut = CreateSut(s => s.ConcurrencyLimit = concurrencyLimit);

        // STAGE 1

        // Act
        sut.Activate();
        factoryCalledBeforeCompletionSignal.Wait(s_waitingTimeout, CancellationToken.None);

        // Assert
        Assert.Equal(concurrencyLimit - 1, Volatile.Read(ref capturedCapacity));

        // STAGE 2

        // Act
        completeJobSignal.Set();
        factoryCalledAfterCompletionSignal.Wait(s_waitingTimeout, CancellationToken.None);

        // Assert
        Assert.Equal(concurrencyLimit, Volatile.Read(ref capturedCapacity));
    }

    [Fact]
    public async Task JobProcessor_WithNoJobsAvailable_EventuallyStopsPolling()
    {
        // Arrange
        int factoryCallCount = 0;

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                Interlocked.Increment(ref factoryCallCount);
                return [];
            });

        using JobProcessor sut = CreateSut();

        // Act
        sut.Activate();
        await WaitTillProcessorIdles();

        int finalFactoryCallCount = Volatile.Read(ref factoryCallCount);
        await WaitForPotentialNextIteration();

        // Assert
        Assert.Equal(finalFactoryCallCount, Volatile.Read(ref factoryCallCount));
    }

    #endregion JOB EXECUTION

    #region PROCESSOR TIMING

    [Fact]
    public async Task JobProcessor_WhenProcessingScheduleIsInProgress_SkipsOverlappingTick()
    {
        // Arrange
        int factoryCallCount = 0;
        var factoryCalledForTheFirstTimeSignal = new ManualResetEventSlim(false);
        var factoryCalledAgainSignal = new ManualResetEventSlim(false);
        var completeFactoryMethodSignal = new ManualResetEventSlim(false);

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                if (Interlocked.Increment(ref factoryCallCount) == 1)
                {
                    factoryCalledForTheFirstTimeSignal.Set();
                }
                else
                {
                    factoryCalledAgainSignal.Set();
                }
                completeFactoryMethodSignal.Wait(s_waitingTimeout, CancellationToken.None);
                return [];
            });

        using JobProcessor sut = CreateSut();

        // STAGE 1

        // Act
        sut.Activate();

        factoryCalledForTheFirstTimeSignal.Wait(s_waitingTimeout, CancellationToken.None);
        await WaitForPotentialNextIteration();

        // Assert
        Assert.Equal(1, Volatile.Read(ref factoryCallCount));

        // STAGE 2

        // Act
        completeFactoryMethodSignal.Set();
        factoryCalledAgainSignal.Wait(s_waitingTimeout, CancellationToken.None);

        // Assert
        Assert.True(Volatile.Read(ref factoryCallCount) >= 2);
    }

    #endregion PROCESSOR TIMING

    #region ERROR HANDLING

    [Fact]
    public async Task JobProcessor_WhenFactoryThrows_RaisesScheduleProcessingErrorAndContinuesProcessing()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Factory error!");

        int factoryCallCount = 0;
        var factoryCalledAfterExceptionSignal = new ManualResetEventSlim(false);

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                if (Interlocked.Increment(ref factoryCallCount) == 1)
                {
                    throw expectedException;
                }
                factoryCalledAfterExceptionSignal.Set();
                return [];
            });

        using JobProcessor sut = CreateSut();

        Exception? capturedException = null;
        var onScheduleProcessingErrorCalledSignal = new ManualResetEventSlim(false);

        sut.OnScheduleProcessingError += (_, ex) =>
        {
            capturedException = ex;
            onScheduleProcessingErrorCalledSignal.Set();
        };

        // STAGE 1

        // Act
        sut.Activate();
        onScheduleProcessingErrorCalledSignal.Wait(s_waitingTimeout, CancellationToken.None);

        // Assert
        Assert.Same(expectedException, capturedException);

        // STAGE 2

        // Act
        factoryCalledAfterExceptionSignal.Wait(s_waitingTimeout, CancellationToken.None);

        // Assert
        Assert.True(Volatile.Read(ref factoryCallCount) >= 2);
    }

    [Fact]
    public async Task JobProcessor_WhenScheduleProcessingErrorSubscriberThrows_IgnoresAndContinuesProcessing()
    {
        // Arrange
        int factoryCallCount = 0;
        var factoryCalledAfterExceptionSignal = new ManualResetEventSlim(false);

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                if (Interlocked.Increment(ref factoryCallCount) == 1)
                {
                    throw new InvalidOperationException("Factory error!");
                }
                factoryCalledAfterExceptionSignal.Set();
                return [];
            });

        using JobProcessor sut = CreateSut();

        sut.OnScheduleProcessingError += (_, _) => throw new Exception("Subscriber error!");

        // Act
        sut.Activate();
        factoryCalledAfterExceptionSignal.Wait(s_waitingTimeout, CancellationToken.None);

        // Assert
        Assert.True(Volatile.Read(ref factoryCallCount) >= 2);
    }

    [Fact]
    public async Task JobProcessor_WhenJobThrows_RaisesJobProcessingErrorAndRestoresCapacity()
    {
        // Arrange
        int concurrencyLimit = 3;

        var jobMock = new Mock<IJob>();
        var completeJobSignal = new ManualResetEventSlim(false);
        var expectedException = new InvalidOperationException("Job error!");

        jobMock
            .Setup(j => j.ExecuteAsync(It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                completeJobSignal.Wait(s_waitingTimeout, CancellationToken.None);
                throw expectedException;
            });

        int factoryCallCount = 0;
        int capturedCapacity = -1;
        bool exceptionCaptured = false;

        var factoryCalledBeforeTheExceptionSignal = new ManualResetEventSlim(false);
        var factoryCalledAfterExceptionSignal = new ManualResetEventSlim(false);

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int maxCount, CancellationToken _) =>
            {
                Volatile.Write(ref capturedCapacity, maxCount);

                if (Interlocked.Increment(ref factoryCallCount) == 1)
                {
                    return [jobMock.Object];
                }

                if (Volatile.Read(ref exceptionCaptured))
                {
                    factoryCalledAfterExceptionSignal.Set();
                }
                else
                {
                    factoryCalledBeforeTheExceptionSignal.Set();
                }

                return [];
            });

        using JobProcessor sut = CreateSut(s => s.ConcurrencyLimit = concurrencyLimit);

        Exception? capturedException = null;
        var onJobProcessingErrorCalledSignal = new ManualResetEventSlim(false);

        sut.OnJobProcessingError += (_, ex) =>
        {
            capturedException = ex;
            Volatile.Write(ref exceptionCaptured, true);
            onJobProcessingErrorCalledSignal.Set();
        };

        // STAGE 1

        // Act
        sut.Activate();
        factoryCalledBeforeTheExceptionSignal.Wait(s_waitingTimeout, CancellationToken.None);

        // Assert
        Assert.Equal(concurrencyLimit - 1, Volatile.Read(ref capturedCapacity));

        // STAGE 2

        // Act
        completeJobSignal.Set();
        onJobProcessingErrorCalledSignal.Wait(s_waitingTimeout, CancellationToken.None);

        // Assert
        Assert.True(Volatile.Read(ref exceptionCaptured));
        Assert.Same(expectedException, capturedException);

        // STAGE 3

        // Act
        factoryCalledAfterExceptionSignal.Wait(s_waitingTimeout, CancellationToken.None);

        // Assert
        Assert.Equal(concurrencyLimit, Volatile.Read(ref capturedCapacity));
    }

    [Fact]
    public async Task JobProcessor_WhenJobProcessingErrorSubscriberThrows_IgnoresAndContinuesProcessing()
    {
        // Arrange
        var jobMock = new Mock<IJob>();
        jobMock
            .Setup(j => j.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Job error!"));

        int factoryCallCount = 0;
        bool exceptionCaptured = false;
        var factoryCalledAfterExceptionSignal = new ManualResetEventSlim(false);

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                if (Interlocked.Increment(ref factoryCallCount) == 1)
                {
                    return [jobMock.Object];
                }

                if (Volatile.Read(ref exceptionCaptured))
                {
                    factoryCalledAfterExceptionSignal.Set();
                }

                return [];
            });

        using JobProcessor sut = CreateSut();

        sut.OnJobProcessingError += (_, ex) =>
        {
            Volatile.Write(ref exceptionCaptured, true);
            throw new Exception("Subscriber error!");
        };

        // Act
        sut.Activate();
        factoryCalledAfterExceptionSignal.Wait(s_waitingTimeout, CancellationToken.None);

        // Assert
        Assert.True(Volatile.Read(ref factoryCallCount) >= 2);
    }

    #endregion ERROR HANDLING

    #region DISPOSAL

    [Fact]
    public async Task JobProcessor_AfterDisposal_StopsProcessing()
    {
        // Arrange
        int factoryCallCount = 0;
        var factoryCalledSignal = new ManualResetEventSlim(false);

        _factoryMock
            .Setup(f => f.CreateJobsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([])
            .Callback(() =>
            {
                Interlocked.Increment(ref factoryCallCount);
                factoryCalledSignal.Set();
            });

        JobProcessor sut = CreateSut();

        // Act
        sut.Activate();
        factoryCalledSignal.Wait(s_waitingTimeout, CancellationToken.None);

        sut.Dispose();
        int callCountAfterDispose = Volatile.Read(ref factoryCallCount);

        await WaitTillProcessorIdles();

        // Assert
        Assert.Equal(callCountAfterDispose, Volatile.Read(ref factoryCallCount));
    }

    #endregion DISPOSAL

    private JobProcessor CreateSut(Action<JobProcessorSettings>? configure = null)
    {
        var settings = new JobProcessorSettings
        {
            ConcurrencyLimit = 100,
            InitialInterval = s_initialInterval,
            IntervalMultiplier = s_intervalMultiplier,
            MaxInterval = s_maxInterval,
            ScheduleProcessingTimeout = TimeSpan.FromSeconds(5),
            JobProcessingTimeout = TimeSpan.FromSeconds(5),
        };
        configure?.Invoke(settings);
        return new JobProcessor(_factoryMock.Object, settings);
    }

    private static async Task WaitForPotentialNextIteration()
    {
        // an arbitrary delay long enough to allow for the processor
        // to execute another tick if it was going to
        TimeSpan timeToWait = s_maxInterval * 2;
        await Task.Delay(timeToWait);
    }

    private static async Task WaitTillProcessorIdles()
    {
        TimeSpan timeToWait = s_initialInterval;

        TimeSpan currentInterval = s_initialInterval;
        while (currentInterval * s_intervalMultiplier <= s_maxInterval)
        {
            currentInterval *= s_intervalMultiplier;
            timeToWait += currentInterval;
        }

        TimeSpan safetyMargin = s_maxInterval;
        await Task.Delay(timeToWait + safetyMargin);
    }

    private static Mock<IJob> CreateInfiniteJob()
    {
        var jobMock = new Mock<IJob>();
        var completeJobSignal = new ManualResetEventSlim(false);
        jobMock
            .Setup(j => j.ExecuteAsync(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                completeJobSignal.Wait(s_waitingTimeout, CancellationToken.None);
                return Task.CompletedTask;
            });
        return jobMock;
    }

    private static Mock<IJob> CreateActualJob(ManualResetEventSlim? jobCompletedSignal = null, int iterations = 50_000)
    {
        var jobMock = new Mock<IJob>();
        jobMock
            .Setup(j => j.ExecuteAsync(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                byte[] hash = Encoding.UTF8.GetBytes("payload");
                for (int i = 0; i < iterations; i++)
                {
                    hash = SHA256.HashData(hash);
                }
                jobCompletedSignal?.Set();
                return Task.CompletedTask;
            });
        return jobMock;
    }
}

