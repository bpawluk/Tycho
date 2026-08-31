using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Processor
{
    internal sealed class JobProcessor : IDisposable
    {
        private readonly IJobFactory _jobFactory;
        private readonly IJobRunner _jobRunner;
        private readonly IProcessingSuspender _processingSuspender;
        private readonly IIntervalCalculator _idleTimeCalculator;

        private readonly CancellationTokenSource _processingCts = new CancellationTokenSource();
        private readonly object _sync = new object();
        private Task? _processingTask;

        private bool WasStarted => _processingTask != null;
        private bool IsRunning => WasStarted && !_processingTask!.IsCompleted && !IsStopped;
        private bool IsStopped { get; set; }

        public event EventHandler<Exception>? OnJobProcessorError;

        public JobProcessor(
            IJobFactory jobFactory,
            JobProcessorSettings jobProcessorSettings)
        {
            _jobFactory = jobFactory;
            _jobRunner = new JobRunner(
                jobProcessorSettings.ConcurrencyLimit,
                jobProcessorSettings.JobProcessingTimeout,
                ReportError);
            _processingSuspender = new ProcessingSuspender();
            _idleTimeCalculator = new IntervalCalculator(
                jobProcessorSettings.InitialInterval,
                jobProcessorSettings.MaxInterval,
                jobProcessorSettings.IntervalMultiplier);
        }

        public JobProcessor(
            IJobFactory jobFactory,
            IJobRunner jobRunner,
            IProcessingSuspender processingSuspender,
            IIntervalCalculator idleTimeCalculator)
        {
            _jobRunner = jobRunner;
            _jobFactory = jobFactory;
            _processingSuspender = processingSuspender;
            _idleTimeCalculator = idleTimeCalculator;
        }

        public void Start()
        {
            lock (_sync)
            {
                if (IsStopped)
                {
                    throw new InvalidOperationException("Processing was stopped.");
                }

                if (WasStarted)
                {
                    throw new InvalidOperationException("Processing already started.");
                }

                _processingTask = Task.Run(Process);
            }
        }

        public void Ping()
        {
            lock (_sync)
            {
                if (IsStopped)
                {
                    throw new InvalidOperationException("Processing was stopped.");
                }

                if (!IsRunning)
                {
                    throw new InvalidOperationException("Processing is not running.");
                }
            }

            _processingSuspender.TryResume();
        }

        public async Task StopAsync()
        {
            Task? processingTask;
            bool shouldCancelProcessing;

            lock (_sync)
            {
                processingTask = _processingTask;
                shouldCancelProcessing = !IsStopped;
                IsStopped = true;
            }

            if (shouldCancelProcessing)
            {
                _processingCts.Cancel();
            }

            if (processingTask != null)
            {
                await processingTask.ConfigureAwait(false);
            }
            await _jobRunner.StopAsync().ConfigureAwait(false);
        }

        private async Task Process()
        {
            try
            {
                await ProcessLoop().ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (_processingCts.IsCancellationRequested)
            {
                // Processor stopped
            }
            catch (Exception exception)
            {
                try
                {
                    OnJobProcessorError?.Invoke(this, exception);
                }
                catch { }
            }
        }

        private async Task ProcessLoop()
        {
            while (true)
            {
                _processingCts.Token.ThrowIfCancellationRequested();

                bool shouldSuspendProcessing;
                try
                {
                    bool workPerformed = await ProcessIteration().ConfigureAwait(false);
                    shouldSuspendProcessing = !workPerformed;
                }
                catch (OperationCanceledException) when (_processingCts.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    shouldSuspendProcessing = true;
                    ReportError(exception);
                }

                if (shouldSuspendProcessing)
                {
                    SuspendResult result = await _processingSuspender.SuspendAsync(_idleTimeCalculator.Current, _processingCts.Token).ConfigureAwait(false);
                    if (result == SuspendResult.Completed)
                    {
                        _idleTimeCalculator.Increase();
                    }
                }
                else
                {
                    _idleTimeCalculator.Reset();
                }
            }
        }

        private async Task<bool> ProcessIteration()
        {
            await _jobRunner.WaitForCapacityAsync(_processingCts.Token).ConfigureAwait(false);

            IJob? job = await _jobFactory.TryCreateJobAsync(_processingCts.Token).ConfigureAwait(false);
            if (job == null)
            {
                return false;
            }

            _jobRunner.Run(job);
            return true;
        }

        private void ReportError(Exception exception)
        {
            try
            {
                OnJobProcessorError?.Invoke(this, exception);
            }
            catch { }
        }

        public void Dispose()
        {
            _processingCts.Dispose();
            _jobRunner.Dispose();
        }
    }
}
