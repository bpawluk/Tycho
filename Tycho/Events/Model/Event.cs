using System;
using Tycho.Identity.Events;

namespace Tycho.Events.Model
{
    internal class Event<TEvent> : AbstractEvent where TEvent : class, IEvent
    {
        internal TEvent Payload { get; }

        internal Event(Guid id, EventIdentity eventId, EventHandlerIdentity handlerId, TEvent payload) : base(id, eventId, handlerId)
        {
            Payload = payload;
        }
    }
}
