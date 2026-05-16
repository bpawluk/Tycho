using System;
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
                ScheduleProcessingTimeout = outboxSettings.MessageProcessingTimeout,
            };

            var outboxJobFactory = new OutboxProcessorJobFactory(internals);
            _jobProcessor = new JobProcessor(outboxJobFactory, jobProcessorSettings);
        }

        public void Initialize() => _outboxActivity.NewEntriesAdded += OnEntriesAdded;

        private void OnEntriesAdded(object _, EventArgs __) => _jobProcessor.Activate();

        public void Dispose()
        {
            _outboxActivity.NewEntriesAdded -= OnEntriesAdded;
            _jobProcessor.Dispose();
        }
    }
}
