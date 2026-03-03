using System;
using System.Collections.Generic;

namespace Tycho.Events.Routing.Sources
{
    internal interface IRouteSource<TEvent> where TEvent : class, IEvent
    {
        IReadOnlyCollection<RoutedEvent> Route(Guid eventId, TEvent eventPayload); 
    }
}
