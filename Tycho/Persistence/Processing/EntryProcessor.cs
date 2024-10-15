using System;
using System.Threading;
using System.Threading.Tasks;
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

        public EntryProcessor(IEventRouter eventRouter, IPayloadSerializer payloadSerializer)
        {
            _eventRouter = eventRouter;
            _payloadSerializer = payloadSerializer;
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
            catch
            {
                // TODO: Log the Exception
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