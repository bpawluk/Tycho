using System;
using Tycho.Events.Routing;
using Tycho.Identity.Events;

namespace Tycho.Events.Model
{
    /// <summary>
    /// Represents a routed Event with a serialized payload.
    /// </summary>
    public class SerializedRoutedEvent : Event
    {
        internal Route Route { get; }

        internal string Payload { get; }

        internal SerializedRoutedEvent(Guid id, Guid publishId, EventIdentity eventId, EventHandlerIdentity handlerId, Route route, string payload) : base(id, publishId, eventId, handlerId)
        {
            Route = route;
            Payload = payload;
        }
    }
}
