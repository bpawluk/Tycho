using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tycho.Events.Dispatching;
using Tycho.Events.Model;
using Tycho.Processor;

namespace Tycho.Events.Inbox
{
    internal class InboxProcessorJob : IJob
    {
        private readonly IInboxConsumer _inbox;
        private readonly IEventDispatcher _dispatcher;
        private readonly ILogger<InboxProcessorJob> _logger;

        private RoutedEvent? _event;

        public InboxProcessorJob(
            IInboxConsumer inbox,
            IEventDispatcher dispatcher,
            ILogger<InboxProcessorJob>? logger = null)
        {
            _inbox = inbox;
            _dispatcher = dispatcher;
            _logger = logger ?? NullLogger<InboxProcessorJob>.Instance;
        }

        public InboxProcessorJob ForEvent(RoutedEvent routedEvent)
        {
            _event = routedEvent;
            return this;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (_event is null)
            {
                _logger.LogWarning("No event assigned for processing. Skipping execution.");
                return;
            }

            try
            {
                await _event!.DispatchWithAsync(_dispatcher, cancellationToken).ConfigureAwait(false);
                // do not call _inbox.MarkAsHandled here as this is a responsibility of ScopedEventHandler
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process inbox entry with ID {entryId}", _event.Id);
                await _inbox.MarkAsFailed(_event.Id, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
