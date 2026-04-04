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

        private readonly SemaphoreSlim _scheduleProcessingSemaphore = new SemaphoreSlim(1, 1);
        private readonly object _timerChangeLock = new object();

        private TimeSpan _currentInterval = Timeout.InfiniteTimeSpan;
        private int _jobsInProgress = 0;

        public event EventHandler<Exception>? OnScheduleProcessingError;
        public event EventHandler<Exception>? OnJobProcessingError;

        public JobProcessor(IJobFactory jobFactory, JobProcessorSettings settings)
        {
            _timer = new Timer(ProcessScheduleAsync, null, Timeout.Infinite, Timeout.Infinite);
            _jobFactory = jobFactory;
            _settings = settings;
        }

        public void Activate() => ResetInterval();

        private async void ProcessScheduleAsync(object? _)
        {
            if (await _scheduleProcessingSemaphore.WaitAsync(0).ConfigureAwait(false))
            {
                try
                {
                    var capacity = _settings.ConcurrencyLimit - Volatile.Read(ref _jobsInProgress);
                    if (capacity > 0)
                    {
                        await StartJobsAsync(capacity).ConfigureAwait(false);
                    }
                }
                catch (Exception exception)
                {
                    try
                    {
                        OnScheduleProcessingError?.Invoke(this, exception);
                    }
                    catch { }
                }
                finally
                {
                    _scheduleProcessingSemaphore.Release();
                }
            }
        }

        private async Task StartJobsAsync(int amount)
        {
            using var cts = new CancellationTokenSource(_settings.ScheduleProcessingTimeout);
            var newJobs = await _jobFactory.CreateJobsAsync(amount, cts.Token).ConfigureAwait(false);

            if (newJobs.Count > 0)
            {
                foreach (var job in newJobs)
                {
                    cts.Token.ThrowIfCancellationRequested();
                    _ = Task.Run(async () => await ProcessJobAsync(job).ConfigureAwait(false));
                }
                ResetInterval();
            }
            else
            {
                IncreaseInterval();
            }
        }

        private async Task ProcessJobAsync(IJob job)
        {
            Interlocked.Increment(ref _jobsInProgress);
            using var cts = new CancellationTokenSource(_settings.JobProcessingTimeout);
            try
            {
                await job.ExecuteAsync(cts.Token).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                try
                {
                    OnJobProcessingError?.Invoke(this, exception);
                }
                catch { }
            }
            finally
            {
                Interlocked.Decrement(ref _jobsInProgress);
            }
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
            _scheduleProcessingSemaphore.Dispose();
        }
    }
}
