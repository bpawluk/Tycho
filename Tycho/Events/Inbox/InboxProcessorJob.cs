using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Processor;

namespace Tycho.Events.Inbox
{
    internal class InboxProcessorJob : IJob
    {
        private readonly IInboxConsumer _inboxConsumer;
        private readonly IInboxEntryHandler _entryHandler;
        private readonly InboxProcessorSettings _settings;

        public InboxProcessorJob(
            IInboxConsumer inboxConsumer,
            IInboxEntryHandler entryHandler,
            InboxProcessorSettings? settings = null)
        {
            _inboxConsumer = inboxConsumer;
            _entryHandler = entryHandler;
            _settings = settings ?? InboxProcessorSettings.Default;
        }

        // TODO: OLD OUTBOX PROCESSING LOGIC

        public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            var entriesRead = await _inboxConsumer.Read(_settings.BatchSize, cancellationToken).ConfigureAwait(false);
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
            var handledSuccessfully = await _entryHandler.TryHandlingEntryAsync(entry, cancellationToken).ConfigureAwait(false);
            if (handledSuccessfully)
            {
                await _inboxConsumer.MarkAsHandled(entry, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await _inboxConsumer.MarkAsFailed(entry, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
