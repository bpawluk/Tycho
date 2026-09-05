using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Tycho.Processor;
using Tycho.Structure;

namespace Tycho.Events.Outbox
{
    internal sealed class OutboxProcessor : IHostedService, IDisposable
    {
        private readonly OutboxActivity _outboxActivity;
        private readonly JobProcessor _jobProcessor;

        public OutboxProcessor(Internals internals, OutboxActivity outboxActivity, OutboxSettings? outboxSettings = null)
        {
            _outboxActivity = outboxActivity;

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
            return _jobProcessor.StopAsync();
        }

        public void Dispose()
        {
            _jobProcessor.Dispose();
        }

        private void OnEntriesAdded(object _, EventArgs __) => _jobProcessor.Ping();
    }
}
