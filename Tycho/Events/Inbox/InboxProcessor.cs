using System;
using Tycho.Processor;

namespace Tycho.Events.Inbox
{
    internal sealed class InboxProcessor : IDisposable
    {
        private readonly InboxActivity _outboxActivity;
        private readonly JobProcessor _jobProcessor;

        public InboxProcessor(
            InboxActivity outboxActivity,
            InboxProcessorJob outboxProcessorJob, 
            InboxProcessorSettings settings)
        {
            var jobProcessorSettings = new JobProcessorSettings(
                settings.InitialPollingInterval,
                settings.PollingIntervalMultiplier,
                settings.MaxPollingInterval,
                settings.ProcessingTimeout);

            _jobProcessor = new JobProcessor(outboxProcessorJob, jobProcessorSettings);
            _outboxActivity = outboxActivity;
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
