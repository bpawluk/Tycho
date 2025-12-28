using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Serialization;
using Tycho.Identities.Providers;

namespace Tycho.Events.Inbox
{
    internal class InboxEntryHandler : IInboxEntryHandler
    {
        private readonly IEventHandlerProvider _handlerProvider;
        private readonly IPayloadSerializer _payloadSerializer;

        public InboxEntryHandler(
            IEventHandlerProvider handlerProvider,
            IPayloadSerializer payloadSerializer)
        {
            _handlerProvider = handlerProvider;
            _payloadSerializer = payloadSerializer;
        }

        public async Task HandleEntryAsync(InboxEntry entry, CancellationToken cancellationToken)
        {
            var handler = _handlerProvider.GetHandler(entry.HandlerId);
            var payload = _payloadSerializer.Deserialize(handler.EventType, entry.Payload);
            await HandleWithReflection(entry.Id, payload, handler, cancellationToken).ConfigureAwait(false);
        }

        private Task HandleWithReflection(Guid eventId, IEvent eventData, IEventHandler eventHandler, CancellationToken cancellationToken)
        {
            var eventContextType = typeof(EventContext<>).MakeGenericType(eventHandler.EventType);
            var eventContext = Activator.CreateInstance(eventContextType, eventId, eventData);

            var handleMethod = eventHandler.GetType().GetMethod(nameof(IEventHandler<IEvent>.Handle));
            var handleResult = handleMethod.Invoke(eventHandler, new object[] { eventContext, cancellationToken }) as Task;

            if (handleResult is null)
            {
                throw new InvalidOperationException($"Failure invoking {eventHandler.GetType().Name} Handle method");
            }

            return handleResult;
        }
    }
}
