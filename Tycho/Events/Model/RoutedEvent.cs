using System;
using Tycho.Events.Routing;
using Tycho.Identity.Events;

namespace Tycho.Events.Model
{
    public class RoutedEvent<TEvent> : AbstractRoutedEvent where TEvent : class, IEvent
    {
        internal TEvent Payload { get; }

        internal RoutedEvent(Guid id, EventIdentity eventId, EventHandlerIdentity handlerId, Route route, TEvent payload) : base(id, eventId, handlerId, route)
        {
            Payload = payload;
        }
    }
}
