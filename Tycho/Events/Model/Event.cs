using System;
using Tycho.Identity.Events;

namespace Tycho.Events.Model
{
    /// <summary>
    /// Base class for Tycho Events.
    /// </summary>
    public abstract class Event
    {
        internal Guid Id { get; }

        internal Guid PublishId { get; }

        internal EventIdentity EventId { get; }

        internal EventHandlerIdentity HandlerId { get; }

        internal Event(Guid id, Guid publishId, EventIdentity eventId, EventHandlerIdentity handlerId)
        {
            Id = id;
            PublishId = publishId;
            EventId = eventId;
            HandlerId = handlerId;
        }
    }
}
