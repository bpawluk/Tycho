using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tycho.Processor;
using Tycho.Structure;

namespace Tycho.Events.Outbox
{
    internal sealed class OutboxProcessor : IHostedService, IDisposable
    {
        private readonly OutboxActivity _outboxActivity;
        private readonly JobProcessor _jobProcessor;
        private readonly ILogger<OutboxProcessor>? _logger;

        public OutboxProcessor(
            Internals internals,
            OutboxActivity outboxActivity,
            OutboxSettings? outboxSettings = null,
            ILogger<OutboxProcessor>? logger = null)
        {
            _outboxActivity = outboxActivity;
            _logger = logger;

            outboxSettings ??= OutboxSettings.Default;
            var jobProcessorSettings = new JobProcessorSettings()
            {
                ConcurrencyLimit = outboxSettings.ConcurrencyLimit,
                InitialInterval = outboxSettings.InitialPollingInterval,
                IntervalMultiplier = outboxSettings.PollingIntervalMultiplier,
                MaxInterval = outboxSettings.MaxPollingInterval,
                JobProcessingTimeout = outboxSettings.MessageProcessingTimeout,
            };

            var outboxJobFactory = new OutboxProcessorJobFactory(internals);
            _jobProcessor = new JobProcessor(outboxJobFactory, jobProcessorSettings);
            _jobProcessor.OnJobProcessorError += OnJobProcessorError;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _outboxActivity.NewEntriesAdded += OnEntriesAdded;
            _jobProcessor.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _outboxActivity.NewEntriesAdded -= OnEntriesAdded;
            return _jobProcessor.StopAsync(cancellationToken);
        }

        public void Dispose()
        {
            _jobProcessor.OnJobProcessorError -= OnJobProcessorError;
            _jobProcessor.Dispose();
        }

        private void OnEntriesAdded(object _, EventArgs __) => _jobProcessor.Ping();

        private void OnJobProcessorError(object _, Exception exception)
        {
            _logger?.LogError(exception, "An error occurred while processing outbox entries.");
        }
    }
}
