using System;
using Tycho.Events.Routing;
using Tycho.Identity.Events;

namespace Tycho.Events.Model
{
    public class SerializedRoutedEvent : Event
    {
        internal Route Route { get; }

        internal string Payload { get; }

        internal SerializedRoutedEvent(Guid id, EventIdentity eventId, EventHandlerIdentity handlerId, Route route, string payload) : base(id, eventId, handlerId)
        {
            Route = route;
            Payload = payload;
        }
    }
}
