using System;
using Tycho.Identity.Events;

namespace Tycho.Events.Model
{
    /// <summary>
    /// Represents an event with a serialized payload.
    /// </summary>
    public class SerializedEvent : EventBase
    {
        internal string Payload { get; }

        internal SerializedEvent(Guid id, Guid publishId, EventIdentity eventId, EventHandlerIdentity handlerId, string payload) : base(id, publishId, eventId, handlerId)
        {
            Payload = payload;
        }
    }
}
