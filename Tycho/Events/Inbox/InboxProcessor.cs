using System;
using Tycho.Processor;
using Tycho.Structure;

namespace Tycho.Events.Inbox
{
    internal sealed class InboxProcessor : IDisposable
    {
        private readonly InboxActivity _inboxActivity;
        private readonly JobProcessor _jobProcessor;

        public InboxProcessor(
            Internals internals,
            InboxActivity inboxActivity,
            InboxSettings? inboxSettings = null)
        {
            _inboxActivity = inboxActivity;

            inboxSettings ??= InboxSettings.Default;
            var jobProcessorSettings = new JobProcessorSettings()
            {
                ConcurrencyLimit = inboxSettings.ConcurrencyLimit,
                InitialInterval = inboxSettings.InitialPollingInterval,
                IntervalMultiplier = inboxSettings.PollingIntervalMultiplier,
                MaxInterval = inboxSettings.MaxPollingInterval,
                JobProcessingTimeout = inboxSettings.MessageProcessingTimeout,
                ScheduleProcessingTimeout = inboxSettings.MessageProcessingTimeout,
            };

            var inboxJobFactory = new InboxProcessorJobFactory(internals);
            _jobProcessor = new JobProcessor(inboxJobFactory, jobProcessorSettings);
        }

        public void Initialize()
        {
            _inboxActivity.NewEntriesAdded += OnEntriesAdded;
            _jobProcessor.Activate();
        }

        private void OnEntriesAdded(object _, EventArgs __) => _jobProcessor.Activate();

        public void Dispose()
        {
            _inboxActivity.NewEntriesAdded -= OnEntriesAdded;
            _jobProcessor.Dispose();
        }
    }
}
