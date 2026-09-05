using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tycho.Events.Broker;
using Tycho.Processor;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Events.Outbox
{
    internal class OutboxProcessorJob : IJob
    {
        private readonly Internals _internals;
        private OutboxEvent? _event;

        public OutboxProcessorJob(Internals internals)
        {
            _internals = internals;
        }

        public OutboxProcessorJob ForEvent(OutboxEvent routedEvent)
        {
            _event = routedEvent;
            return this;
        }

        [EntryPoint]
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await using AsyncServiceScope scope = _internals.CreateAsyncScope();
            ILogger<OutboxProcessorJob>? logger = scope.ServiceProvider.GetService<ILogger<OutboxProcessorJob>>();

            if (_event is null)
            {
                logger?.LogWarning("No event assigned for processing. Skipping execution.");
                return;
            }

            IOutboxConsumer outbox = scope.ServiceProvider.GetRequiredService<IOutboxConsumer>();

            try
            {
                IEventBroker broker = scope.ServiceProvider.GetRequiredService<IEventBroker>();
                await broker.DeliverAsync(_event.RoutedEvent, cancellationToken).ConfigureAwait(false);

                bool markedAsDelivered = await outbox
                    .MarkAsDeliveredAsync(_event.ClaimId, cancellationToken)
                    .ConfigureAwait(false);

                if (!markedAsDelivered)
                {
                    logger?.LogWarning("Failed to mark outbox entry with ID {entryId} as delivered for claim {claimId}", _event.EventId, _event.ClaimId);
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to deliver outbox entry with ID {entryId}", _event.EventId);

                bool markedAsFailed = await outbox
                    .MarkAsFailedAsync(_event.ClaimId, cancellationToken)
                    .ConfigureAwait(false);

                if (!markedAsFailed)
                {
                    logger?.LogWarning("Failed to mark outbox entry with ID {entryId} as failed for claim {claimId}", _event.EventId, _event.ClaimId);
                }
            }
        }
    }
}
