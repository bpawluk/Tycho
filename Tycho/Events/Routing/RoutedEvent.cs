using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Broker;
using Tycho.Events.Dispatching;
using Tycho.Events.Routing.Routes;
using Tycho.Identity.Events;

namespace Tycho.Events.Routing
{
    internal abstract class RoutedEvent
    {
        public Guid Id { get; }

        public Route Route { get; }

        public EventHandlerIdentity HandlerId { get; }


        public RoutedEvent(Guid id, Route route, EventHandlerIdentity handlerId)
        {
            Id = id;
            Route = route;
            HandlerId = handlerId;
        }

        public abstract Task DeliverAsync(IEventBroker broker, CancellationToken cancellationToken);

        public abstract Task DispatchAsync(IEventDispatcher dispatcher, CancellationToken cancellationToken);
    }

    internal class RoutedEvent<TEvent> : RoutedEvent where TEvent : class, IEvent
    {
        public TEvent Payload { get; }

        public RoutedEvent(Guid id, EventHandlerIdentity handlerId, TEvent payload) : base(id, Route.Create(), handlerId)
        {
            Payload = payload;
        }

        public RoutedEvent(Guid id, Route route, EventHandlerIdentity handlerId, TEvent payload) : base(id, route, handlerId)
        {
            Payload = payload;
        }

        public override Task DeliverAsync(IEventBroker broker, CancellationToken cancellationToken)
        {
            return broker.DeliverAsync(this, cancellationToken);
        }

        public override Task DispatchAsync(IEventDispatcher dispatcher, CancellationToken cancellationToken)
        {
            return dispatcher.DispatchAsync(this, cancellationToken);
        }
    }
}
