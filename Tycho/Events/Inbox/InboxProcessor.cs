using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Tycho.Processor;
using Tycho.Structure;

namespace Tycho.Events.Inbox
{
    internal sealed class InboxProcessor : IHostedService, IDisposable
    {
        private readonly InboxActivity _inboxActivity;
        private readonly JobProcessor _jobProcessor;

        public InboxProcessor(Internals internals, InboxActivity inboxActivity, InboxSettings? inboxSettings = null)
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
            };

            var inboxJobFactory = new InboxProcessorJobFactory(internals);
            _jobProcessor = new JobProcessor(inboxJobFactory, jobProcessorSettings);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _inboxActivity.NewEntriesAdded += OnEntriesAdded;
            _jobProcessor.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _inboxActivity.NewEntriesAdded -= OnEntriesAdded;
            return _jobProcessor.StopAsync();
        }

        public void Dispose()
        {
            _jobProcessor.Dispose();
        }

        private void OnEntriesAdded(object _, EventArgs __) => _jobProcessor.Ping();
    }
}
