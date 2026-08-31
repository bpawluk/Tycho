using System;
using System.Threading.Tasks;
using Tycho.Processor;
using Tycho.Structure;

namespace Tycho.Events.Outbox
{
    internal sealed class OutboxProcessor : IDisposable
    {
        private readonly OutboxActivity _outboxActivity;
        private readonly JobProcessor _jobProcessor;

        public OutboxProcessor(
            Internals internals,
            OutboxActivity outboxActivity,
            OutboxSettings? outboxSettings = null)
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

        public Task StartAsync()
        {
            _outboxActivity.NewEntriesAdded += OnEntriesAdded;
            _jobProcessor.Start();
            return Task.CompletedTask;
        }

        private void OnEntriesAdded(object _, EventArgs __) => _jobProcessor.Ping();

        public Task StopAsync()
        {
            _outboxActivity.NewEntriesAdded -= OnEntriesAdded;
            return _jobProcessor.StopAsync();
        }

        public void Dispose()
        {
            _jobProcessor.Dispose();
        }
    }
}
