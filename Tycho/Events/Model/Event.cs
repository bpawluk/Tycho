using System;
using Tycho.Identity.Events;

namespace Tycho.Events.Model
{
    public abstract class Event
    {
        internal Guid Id { get; }

        internal EventIdentity EventId { get; }

        internal EventHandlerIdentity HandlerId { get; }

        internal Event(Guid id, EventIdentity eventId, EventHandlerIdentity handlerId)
        {
            Id = id;
            EventId = eventId;
            HandlerId = handlerId;
        }
    }
}
