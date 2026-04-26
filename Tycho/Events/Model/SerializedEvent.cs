using System;
using Tycho.Identity.Events;

namespace Tycho.Events.Model
{
    public class SerializedEvent : AbstractEvent
    {
        internal string Payload { get; }

        internal SerializedEvent(Guid id, EventIdentity eventId, EventHandlerIdentity handlerId, string payload) : base(id, eventId, handlerId)
        {
            Payload = payload;
        }
    }
}
