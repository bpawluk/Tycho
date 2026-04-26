using System;
using Tycho.Events.Routing;
using Tycho.Identity.Events;

namespace Tycho.Events.Model
{
    public class AbstractRoutedEvent : AbstractEvent
    {
        internal Route Route { get; }

        internal AbstractRoutedEvent(Guid id, EventIdentity eventId, EventHandlerIdentity handlerId, Route route) : base(id, eventId, handlerId)
        {
            Route = route;
        }
    }
}
