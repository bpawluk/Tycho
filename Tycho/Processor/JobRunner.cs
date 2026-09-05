using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Processor
{
    internal sealed class JobRunner : IJobRunner, IDisposable
    {
        private readonly Action<Exception> _onError;
        private readonly TimeSpan _jobTimeout;
        private readonly SemaphoreSlim _capacity;

        private readonly CancellationTokenSource _processingCts = new CancellationTokenSource();
        private readonly HashSet<Task> _runningJobs = new HashSet<Task>();
        private readonly object _sync = new object();

        private Task? _stopTask;

        public JobRunner(int maximalConcurrency, TimeSpan jobTimeout, Action<Exception> onError)
        {
            if (maximalConcurrency <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximalConcurrency));
            }

            if (jobTimeout <= TimeSpan.Zero && jobTimeout != Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentOutOfRangeException(nameof(jobTimeout));
            }

            _jobTimeout = jobTimeout;
            _onError = onError ?? throw new ArgumentNullException(nameof(onError));
            _capacity = new SemaphoreSlim(maximalConcurrency, maximalConcurrency);
        }

        public async Task WaitForCapacityAsync(CancellationToken cancellationToken)
        {
            await _capacity.WaitAsync(cancellationToken).ConfigureAwait(false);
            _capacity.Release();
        }

        public void Run(IJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            if (!_capacity.Wait(0))
            {
                throw new InvalidOperationException("No job execution capacity is available.");
            }

            Task task;

            try
            {
                task = Task.Run(() => RunJobAsync(job));
            }
            catch
            {
                _capacity.Release();
                throw;
            }

            lock (_sync)
            {
                _runningJobs.Add(task);
            }
            _ = RemoveWhenCompletedAsync(task);
        }

        public Task StopAsync()
        {
            Task stopTask;

            lock (_sync)
            {
                if (_stopTask != null)
                {
                    return _stopTask;
                }

                _stopTask = Task.WhenAll(_runningJobs.ToArray());
                stopTask = _stopTask;
            }

            try
            {
                _processingCts.Cancel();
            }
            catch (Exception exception)
            {
                ReportError(exception);
            }

            return stopTask;
        }

        private async Task RunJobAsync(IJob job)
        {
            using CancellationTokenSource timeoutCts = new CancellationTokenSource();
            using CancellationTokenSource jobCts = CancellationTokenSource.CreateLinkedTokenSource(_processingCts.Token, timeoutCts.Token);

            if (_jobTimeout != Timeout.InfiniteTimeSpan)
            {
                timeoutCts.CancelAfter(_jobTimeout);
            }

            try
            {
                await job.ExecuteAsync(jobCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (_processingCts.IsCancellationRequested)
            {
                // Job Runner stopped
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                ReportTimeout();
            }
            catch (Exception exception)
            {
                ReportError(exception);
            }
            finally
            {
                _capacity.Release();
            }
        }

        private async Task RemoveWhenCompletedAsync(Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            finally
            {
                lock (_sync)
                {
                    _runningJobs.Remove(task);
                }
            }
        }

        private void ReportTimeout()
        {
            ReportError(new TimeoutException($"Job execution exceeded the timeout of {_jobTimeout}."));
        }

        private void ReportError(Exception exception)
        {
            try
            {
                _onError(exception);
            }
            catch { }
        }

        public void Dispose()
        {
            _capacity.Dispose();
            _processingCts.Dispose();
        }
    }
}
