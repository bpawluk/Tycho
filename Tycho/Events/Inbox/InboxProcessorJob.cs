using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tycho.Processor;

namespace Tycho.Events.Inbox
{
    internal class InboxProcessorJob : IJob
    {
        private readonly IInboxConsumer _outboxConsumer;
        private readonly InboxProcessorSettings _settings;
        private readonly ILogger<InboxProcessorJob> _logger;   

        public InboxProcessorJob(
            IInboxConsumer outboxConsumer,
            InboxProcessorSettings? settings = null,
            ILogger<InboxProcessorJob>? logger = null)
        {
            _outboxConsumer = outboxConsumer;
            _settings = settings ?? InboxProcessorSettings.Default;
            _logger = logger ?? NullLogger<InboxProcessorJob>.Instance;
        }

        public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            var entriesRead = await _outboxConsumer.Read(_settings.BatchSize, cancellationToken).ConfigureAwait(false);
            if (entriesRead.Any())
            {
                var deliveryTasks = entriesRead.Select(entry => HandleEntryAsync(entry, cancellationToken));
                await Task.WhenAll(deliveryTasks).ConfigureAwait(false);
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task HandleEntryAsync(InboxEntry entry, CancellationToken cancellationToken)
        {
            try 
            {
                await _outboxConsumer.MarkAsHandled(entry, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Failed to handle inbox entry with ID {entryId}", entry.Id);
                await _outboxConsumer.MarkAsFailed(entry, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
