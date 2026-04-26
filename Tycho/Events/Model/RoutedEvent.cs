using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Dispatching;
using Tycho.Events.Routing;
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

        internal abstract Task DispatchWithAsync(IEventDispatcher dispatcher, CancellationToken cancellationToken);
    }

    public class RoutedEvent<TEvent> : RoutedEvent where TEvent : class, IEvent
    {
        internal TEvent Payload { get; }

        internal RoutedEvent(Guid id, EventIdentity eventId, EventHandlerIdentity handlerId, Route route, TEvent payload) : base(id, eventId, handlerId, route)
        {
            Payload = payload;
        }

        internal override Task DispatchWithAsync(IEventDispatcher dispatcher, CancellationToken cancellationToken)
        {
            return dispatcher.DispatchAsync(this, cancellationToken);
        }
    }
}
