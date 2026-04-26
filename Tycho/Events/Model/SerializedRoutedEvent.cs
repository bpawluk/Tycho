using System;
using Tycho.Events.Routing;
using Tycho.Identity.Events;

namespace Tycho.Events.Model
{
    public class SerializedRoutedEvent : AbstractRoutedEvent
    {
        internal string Payload { get; }

        internal SerializedRoutedEvent(Guid id, EventIdentity eventId, EventHandlerIdentity handlerId, Route route, string payload) : base(id, eventId, handlerId, route)
        {
            Payload = payload;
        }
    }
}
