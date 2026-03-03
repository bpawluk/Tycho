using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tycho.Events.Routing;
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

        private async Task DeliverEntryAsync(RoutedEvent routedEvent, CancellationToken cancellationToken)
        {
            try 
            {
                await routedEvent.DeliverAsync(_eventRouter, cancellationToken).ConfigureAwait(false);
                await _outboxConsumer.MarkAsDelivered(routedEvent.Id, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Failed to deliver outbox entry with ID {entryId}", routedEvent.Id);
                await _outboxConsumer.MarkAsFailed(routedEvent.Id, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
