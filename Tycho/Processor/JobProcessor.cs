using System;
using System.Threading;

namespace Tycho.Processor
{
    internal sealed class JobProcessor : IDisposable
    {
        private readonly IJob _job;
        private readonly JobProcessorSettings _settings;

        private readonly Timer _timer;

        private readonly object _timerChangeLock;
        private readonly SemaphoreSlim _processingSemaphore;

        private TimeSpan _currentInterval = Timeout.InfiniteTimeSpan;

        public event EventHandler<Exception>? OnError;

        public JobProcessor(IJob job, JobProcessorSettings settings)
        {
            _job = job;
            _settings = settings;

            _timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);

            _timerChangeLock = new object();
            _processingSemaphore = new SemaphoreSlim(1, 1);
        }

        public void Activate() => ResetInterval();

        private async void TimerCallback(object? _)
        {
            if (await _processingSemaphore.WaitAsync(0).ConfigureAwait(false))
            {
                try
                {
                    using var cancellationTokenSource = new CancellationTokenSource(_settings.ProcessingTimeout);

                    var processed = await _job.ExecuteAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                    if (processed)
                    {
                        ResetInterval();
                    }
                    else
                    {
                        IncreaseInterval();
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        OnError?.Invoke(this, ex);
                    }
                    catch { }
                }
                finally
                {
                    _processingSemaphore.Release();
                }
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
                    // stop processing
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
            _processingSemaphore.Dispose();
        }
    }
}
