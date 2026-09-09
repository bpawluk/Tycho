using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tycho.Processor;
using Tycho.Structure;

namespace Tycho.Events.Inbox
{
    internal sealed class InboxProcessor : IHostedService, IDisposable
    {
        private readonly InboxActivity _inboxActivity;
        private readonly JobProcessor _jobProcessor;
        private readonly ILogger<InboxProcessor>? _logger;

        public InboxProcessor(
            Internals internals,
            InboxActivity inboxActivity,
            InboxSettings? inboxSettings = null,
            ILogger<InboxProcessor>? logger = null)
        {
            _inboxActivity = inboxActivity;
            _logger = logger;

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
            _jobProcessor.OnJobProcessorError += OnJobProcessorError;
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
            _logger?.LogError(exception, "An error occurred while processing inbox entries.");
        }
    }
}
