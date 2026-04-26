using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tycho.Events.Broker;
using Tycho.Events.Model;
using Tycho.Processor;

namespace Tycho.Events.Outbox
{
    internal class OutboxProcessorJob : IJob
    {
        private readonly IOutboxConsumer _outbox;
        private readonly IEventBroker _broker;
        private readonly ILogger<OutboxProcessorJob> _logger;

        private SerializedRoutedEvent? _event;

        public OutboxProcessorJob(
            IOutboxConsumer outbox,
            IEventBroker broker,
            ILogger<OutboxProcessorJob>? logger = null)
        {
            _outbox = outbox;
            _broker = broker;
            _logger = logger ?? NullLogger<OutboxProcessorJob>.Instance;
        }

        public OutboxProcessorJob ForEvent(SerializedRoutedEvent routedEvent)
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
                await _broker.DeliverAsync(_event, cancellationToken).ConfigureAwait(false);
                await _outbox.MarkAsDelivered(_event.Id, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deliver outbox entry with ID {entryId}", _event.Id);
                await _outbox.MarkAsFailed(_event.Id, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
