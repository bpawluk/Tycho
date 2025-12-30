using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Handling;
using Tycho.Events.Serialization;
using Tycho.Registry;

namespace Tycho.Events.Inbox
{
    internal class InboxEntryHandler : IInboxEntryHandler
    {
        private readonly IEventHandlerRegistry _handlerRegistry;
        private readonly IPayloadSerializer _payloadSerializer;

        public InboxEntryHandler(
            IEventHandlerRegistry handlerRegistry,
            IPayloadSerializer payloadSerializer)
        {
            _handlerRegistry = handlerRegistry;
            _payloadSerializer = payloadSerializer;
        }

        public async Task HandleEntryAsync(InboxEntry entry, CancellationToken cancellationToken)
        {
            var handler = _handlerRegistry.GetHandler(entry.HandlerId);
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

    internal class OtherInboxEntryHandler : IInboxEntryHandler
    {
        private readonly IEventHandlerRegistry _handlerRegistry;
        private readonly IPayloadSerializer _payloadSerializer;

        public OtherInboxEntryHandler(
            IEventHandlerRegistry handlerRegistry,
            IPayloadSerializer payloadSerializer)
        {
            _handlerRegistry = handlerRegistry;
            _payloadSerializer = payloadSerializer;
        }

        public async Task HandleEntryAsync(InboxEntry entry, CancellationToken cancellationToken)
        {
            switch (_handlerRegistry.GetHandler(entry.HandlerId))
            {
                case IEventHandler<TestEvent> testEventHandler:
                    await HandleAs(entry, testEventHandler, cancellationToken);
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Missing handling logic for event handler with ID {entry.HandlerId}");
            }
        }

        private async Task HandleAs<TEvent>(
            InboxEntry entry, 
            IEventHandler<TEvent> handler,  
            CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            var deserializedPyaload = _payloadSerializer.Deserialize<TEvent>(entry.Payload);
            var context = new EventContext<TEvent>(entry.Id, deserializedPyaload);
            await handler.Handle(context, cancellationToken);
        }
    }

    internal class TestEvent : IEvent
    {
    }
}

