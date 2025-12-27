using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Payload;
using Tycho.Processor;

namespace Tycho.Events.Outbox
{
    internal class OutboxProcessorJob : IJob
    {
        private readonly IOutboxConsumer _outboxConsumer;
        private readonly IEventRouter _eventRouter;
        private readonly OutboxProcessorSettings _settings;
        private readonly ILogger<OutboxProcessorJob> _logger;   

        public OutboxProcessorJob(
            IOutboxConsumer outboxConsumer,
            IEventRouter eventRouter,
            OutboxProcessorSettings? settings = null,
            ILogger<OutboxProcessorJob>? logger = null)
        {
            _outboxConsumer = outboxConsumer;
            _eventRouter = eventRouter;
            _settings = settings ?? OutboxProcessorSettings.Default;
            _logger = logger ?? NullLogger<OutboxProcessorJob>.Instance;
        }

        public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            var entriesRead = await _outboxConsumer.Read(_settings.BatchSize, cancellationToken).ConfigureAwait(false);
            if (entriesRead.Any())
            {
                var deliveryTasks = entriesRead.Select(entry => DeliverEntryAsync(entry, cancellationToken));
                await Task.WhenAll(deliveryTasks).ConfigureAwait(false);
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task DeliverEntryAsync(OutboxEntry entry, CancellationToken cancellationToken)
        {
            try 
            {
                var routedEvent = new RoutedEvent(entry.Id, entry.Payload, entry.Route);
                await _eventRouter.DeliverAsync(routedEvent, cancellationToken).ConfigureAwait(false);
                await _outboxConsumer.MarkAsDelivered(entry.Id, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Failed to deliver outbox entry with ID {entryId}", entry.Id);
                await _outboxConsumer.MarkAsFailed(entry.Id, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
