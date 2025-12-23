using System;
using System.Collections.Generic;
using Tycho.Events.Routing.Payload;

namespace Tycho.Events.Routing.Sources
{
    internal interface IRouteSource<TEvent> where TEvent : class, IEvent
    {
        IReadOnlyCollection<IRoutedEvent<IEvent>> GetRoutes(Guid eventId, TEvent eventPayload); 
    }
}
