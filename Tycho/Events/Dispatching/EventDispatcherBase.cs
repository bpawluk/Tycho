using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Serialization;
using Tycho.Utils;

namespace Tycho.Events.Dispatching
{
    [ReferencedBySourceGenerator]
    public abstract class EventDispatcherBase : IEventDispatcher
    {
        private readonly IPayloadSerializer _payloadSerializer;

        [ReferencedBySourceGenerator]
        public EventDispatcherBase(IPayloadSerializer payloadSerializer)
        {
            _payloadSerializer = payloadSerializer;
        }

        [ReferencedBySourceGenerator]
        public abstract Task Dispatch(
            Guid eventId,
            object eventPayload,
            IEventHandler eventHandler,
            CancellationToken cancellationToken);

        [ReferencedBySourceGenerator]
        protected async Task DispatchAs<TEvent>(
            Guid eventId,
            object eventPayload,
            IEventHandler<TEvent> eventHandler,
            CancellationToken cancellationToken) where TEvent : class, IEvent
        {
            var deserializedPayload = _payloadSerializer.Deserialize<TEvent>(eventPayload);
            var eventContext = new EventContext<TEvent>(eventId, deserializedPayload);
            await eventHandler.HandleAsync(eventContext, cancellationToken);
        }
    }
}
