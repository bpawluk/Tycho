using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tycho.Events;
using Tycho.Events.Routing;

namespace Tycho.Persistence.Processing
{
    internal interface IEntryProcessor
    {
        Task<bool> Process(OutboxEntry entry);
    }

    internal class EntryProcessor : IEntryProcessor
    {
        private readonly IEventRouter _eventRouter;
        private readonly IPayloadSerializer _payloadSerializer;
        private readonly ILogger<EntryProcessor> _logger;

        public EntryProcessor(
            IEventRouter eventRouter, 
            IPayloadSerializer payloadSerializer,
            ILogger<EntryProcessor>? logger = null)
        {
            _eventRouter = eventRouter;
            _payloadSerializer = payloadSerializer;
            _logger = logger ?? NullLogger<EntryProcessor>.Instance;
        }

        public async Task<bool> Process(OutboxEntry entry)
        {
            try
            {
                var eventHandler = _eventRouter.FindHandler(entry.HandlerIdentity);
                if (eventHandler is null)
                {
                    throw new InvalidOperationException($"Missing event handler with ID {entry.HandlerIdentity}");
                }

                var eventData = _payloadSerializer.Deserialize(eventHandler.EventType, entry.Payload);
                await HandleWithReflection(eventData, eventHandler).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process entry {entryId}", entry.Id);
                return false;
            }
        }

        private Task HandleWithReflection(IEvent eventData, IEventHandler eventHandler)
        {
            var handleMethod = eventHandler.GetType().GetMethod(nameof(IEventHandler<IEvent>.Handle));

            var handleResult = handleMethod.Invoke(
                eventHandler,
                new object[] {
                    eventData,
                    CancellationToken.None })
                as Task;

            if (handleResult is null)
            {
                throw new InvalidOperationException($"Failure invoking {eventHandler.GetType().Name} Handle method");
            }

            return handleResult;
        }
    }
}