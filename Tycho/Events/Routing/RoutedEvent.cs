using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Broker;
using Tycho.Events.Dispatching;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;

namespace Tycho.Events.Routing
{
    public abstract class RoutedEvent
    {
        internal Guid Id { get; }

        internal EventIdentity EventId { get; }

        internal EventHandlerIdentity HandlerId { get; }

        internal Route Route { get; }

        internal RoutedEvent(Guid id, EventIdentity eventId, EventHandlerIdentity handlerId, Route route)
        {
            Id = id;
            EventId = eventId;
            HandlerId = handlerId;
            Route = route;
        }

        internal abstract Task DeliverAsync(IEventBroker broker, CancellationToken cancellationToken);

        internal abstract Task DispatchAsync(IEventDispatcher dispatcher, CancellationToken cancellationToken);

        internal abstract string SerializePayload(IPayloadSerializer serializer);
    }

    public class RoutedEvent<TEvent> : RoutedEvent where TEvent : class, IEvent
    {
        internal TEvent Payload { get; }

        internal RoutedEvent(Guid id, EventHandlerIdentity handlerId, TEvent payload) : base(id, EventIdentity.Create<TEvent>(), handlerId, Route.Create())
        {
            Payload = payload;
        }

        internal RoutedEvent(Guid id, EventHandlerIdentity handlerId, Route route, TEvent payload) : base(id, EventIdentity.Create<TEvent>(), handlerId, route)
        {
            Payload = payload;
        }

        internal override Task DeliverAsync(IEventBroker broker, CancellationToken cancellationToken)
        {
            return broker.DeliverAsync(this, cancellationToken);
        }

        internal override Task DispatchAsync(IEventDispatcher dispatcher, CancellationToken cancellationToken)
        {
            return dispatcher.DispatchAsync(this, cancellationToken);
        }

        internal override string SerializePayload(IPayloadSerializer serializer)
        {
            return serializer.Serialize(Payload);
        }

        public static RoutedEvent<TEvent> Create(Guid id, string handlerIdString, string routeString, TEvent payload)
        {
            var handlerId = EventHandlerIdentity.Parse(handlerIdString);
            var route = Route.Parse(routeString);
            return new RoutedEvent<TEvent>(id, handlerId, route, payload);
        }
    }
}
