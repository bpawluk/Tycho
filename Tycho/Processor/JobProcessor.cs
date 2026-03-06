using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Processor
{
    internal sealed class JobProcessor : IDisposable
    {
        private readonly Timer _timer;

        private readonly IJobFactory _jobFactory;
        private readonly JobProcessorSettings _settings;

        private readonly SemaphoreSlim _timerElapsedSemaphore = new SemaphoreSlim(1, 1);
        private readonly object _timerChangeLock = new object();

        private TimeSpan _currentInterval = Timeout.InfiniteTimeSpan;
        private int _jobsInProgress = 0;

        public event EventHandler<Exception>? OnScheduleProcessingError;
        public event EventHandler<Exception>? OnJobProcessingError;

        public JobProcessor(IJobFactory jobFactory, JobProcessorSettings settings)
        {
            _timer = new Timer(TimerElapsed, null, Timeout.Infinite, Timeout.Infinite);
            _jobFactory = jobFactory;
            _settings = settings;
        }

        public void Activate() => ResetInterval();

        private async void TimerElapsed(object? _)
        {
            if (await _timerElapsedSemaphore.WaitAsync(0).ConfigureAwait(false))
            {
                using var cts = new CancellationTokenSource(_settings.ScheduleProcessingTimeout);
                try
                {
                    var capacity = _settings.ConcurrencyLimit - Volatile.Read(ref _jobsInProgress);
                    if (capacity > 0)
                    {
                        var newJobs = await _jobFactory
                            .CreateJobsAsync(capacity, cts.Token)
                            .ConfigureAwait(false);

                        if (newJobs.Count > 0)
                        {
                            foreach (var job in newJobs)
                            {
                                StartJob(job);
                            }
                            ResetInterval();
                        }
                        else
                        {
                            IncreaseInterval();
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        OnScheduleProcessingError?.Invoke(this, ex);
                    }
                    catch { }
                }
                finally
                {
                    _timerElapsedSemaphore.Release();
                }
            }
        }

        private void StartJob(IJob job)
        {
            Interlocked.Increment(ref _jobsInProgress);
            Task.Run(async () =>
            {
                using var cts = new CancellationTokenSource(_settings.JobProcessingTimeout);
                try
                {
                    await job.ExecuteAsync(cts.Token);
                }
                catch (Exception ex)
                {
                    try
                    {
                        OnJobProcessingError?.Invoke(this, ex);
                    }
                    catch { }
                }
                finally
                {
                    Interlocked.Decrement(ref _jobsInProgress);
                }
            });
        }

        private void ResetInterval()
        {
            lock (_timerChangeLock)
            {
                if (_currentInterval != _settings.InitialInterval)
                {
                    _currentInterval = _settings.InitialInterval;
                    _timer.Change(TimeSpan.Zero, _currentInterval);
                }
            }
        }

        private void IncreaseInterval()
        {
            lock (_timerChangeLock)
            {
                var newInterval = _currentInterval * _settings.IntervalMultiplier;

                if (newInterval > _settings.MaxInterval)
                {
                    newInterval = Timeout.InfiniteTimeSpan;
                }

                _currentInterval = newInterval;
                _timer.Change(_currentInterval, _currentInterval);
            }
        }

        public void Dispose()
        {
            using var timerDisposal = new ManualResetEvent(false);
            _timer.Dispose(timerDisposal);

            timerDisposal.WaitOne();
            _timerElapsedSemaphore.Dispose();
        }
    }
}
