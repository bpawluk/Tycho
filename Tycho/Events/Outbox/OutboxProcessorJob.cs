using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tycho.Events.Broker;
using Tycho.Events.Model;
using Tycho.Processor;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Events.Outbox
{
    internal class OutboxProcessorJob : IJob
    {
        private readonly Internals _internals;
        private SerializedRoutedEvent? _event;

        public OutboxProcessorJob(Internals internals)
        {
            _internals = internals;
        }

        public OutboxProcessorJob ForEvent(SerializedRoutedEvent routedEvent)
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
                await broker.DeliverAsync(_event, cancellationToken).ConfigureAwait(false);
                await outbox.MarkAsDelivered(_event.Id, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to deliver outbox entry with ID {entryId}", _event.Id);
                await outbox.MarkAsFailed(_event.Id, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
