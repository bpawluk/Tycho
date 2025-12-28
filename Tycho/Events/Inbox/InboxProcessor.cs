using System;
using Tycho.Processor;

namespace Tycho.Events.Inbox
{
    internal sealed class InboxProcessor : IDisposable
    {
        private readonly InboxActivity _inboxActivity;
        private readonly JobProcessor _jobProcessor;

        public InboxProcessor(
            InboxActivity inboxActivity,
            InboxProcessorJob outboxProcessorJob, 
            InboxProcessorSettings? settings = null)
        {
            settings ??= InboxProcessorSettings.Default;

            var jobProcessorSettings = new JobProcessorSettings(
                settings.InitialPollingInterval,
                settings.PollingIntervalMultiplier,
                settings.MaxPollingInterval,
                settings.ProcessingTimeout);

            _jobProcessor = new JobProcessor(outboxProcessorJob, jobProcessorSettings);
            _inboxActivity = inboxActivity;
        }

        public void Initialize() => _inboxActivity.NewEntriesAdded += OnEntriesAdded;

        private void OnEntriesAdded(object _, EventArgs __) => _jobProcessor.Activate();

        public void Dispose()
        {
            _inboxActivity.NewEntriesAdded -= OnEntriesAdded;
            _jobProcessor.Dispose();
        }
    }
}
