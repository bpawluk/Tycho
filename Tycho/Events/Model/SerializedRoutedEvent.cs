using System;
using Tycho.Identity.Events;
using Tycho.Identity.Structure;

namespace Tycho.Events.Model
{
    /// <summary>
    /// Represents a routed event with a serialized payload.
    /// </summary>
    public class SerializedRoutedEvent : SerializedEvent
    {
        internal InstanceIdentity DestinationId { get; }

        internal SerializedRoutedEvent(Guid id, Guid publishId, EventIdentity eventId, EventHandlerIdentity handlerId, InstanceIdentity destinationId, string payload) : base(id, publishId, eventId, handlerId, payload)
        {
            DestinationId = destinationId;
        }
    }
}
