using System;
using Tycho.Events.Outbox;
using Tycho.Processor;

namespace Tycho.Events.Outbox
{
    internal sealed class OutboxProcessor : IDisposable
    {
        private readonly OutboxActivity _outboxActivity;
        private readonly JobProcessor _jobProcessor;

        public OutboxProcessor(
            OutboxActivity outboxActivity,
            OutboxProcessorJobFactory outboxJobFactory,
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
