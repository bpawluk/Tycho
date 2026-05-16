using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;

namespace Tycho.Events.Model
{
    public abstract class RoutedEvent : Event
    {
        internal Route Route { get; }

        internal RoutedEvent(Guid id, EventIdentity eventId, EventHandlerIdentity handlerId, Route route) : base(id, eventId, handlerId)
        {
            Route = route;
        }

        internal abstract string SerializePayloadWith(IPayloadSerializer serializer);

        internal abstract IEventHandler GetHandlerFrom(IEventHandlerProvider provider);

        internal abstract Task HandleWith(IEventHandler handler, CancellationToken cancellationToken);
    }

    public class RoutedEvent<TEvent> : RoutedEvent where TEvent : class, IEvent
    {
        internal TEvent Payload { get; }

        internal RoutedEvent(Guid id, EventIdentity eventId, EventHandlerIdentity handlerId, Route route, TEvent payload) : base(id, eventId, handlerId, route)
        {
            Payload = payload;
        }

        internal override string SerializePayloadWith(IPayloadSerializer serializer)
        {
            return serializer.Serialize(Payload);
        }

        internal override IEventHandler GetHandlerFrom(IEventHandlerProvider provider)
        {
            return provider.GetHandler<TEvent>(HandlerId);
        }

        internal override async Task HandleWith(IEventHandler handler, CancellationToken cancellationToken)
        {
            if (handler is IEventHandler<TEvent> typedHandler)
            {
                var context = new EventContext<TEvent>(Id, Payload);
                await typedHandler.HandleAsync(context, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                throw new ArgumentException($"Handler is not of type IEventHandler<{typeof(TEvent).Name}>");
            }
        }
    }
}
