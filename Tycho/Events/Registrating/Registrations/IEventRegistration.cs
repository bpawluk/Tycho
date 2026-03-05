using System;
using System.Collections.Generic;
using Tycho.Events.Routing;

namespace Tycho.Events.Registrating.Registrations
{
    internal interface IEventRegistration<TEvent> where TEvent : class, IEvent
    {
        IReadOnlyCollection<RoutedEvent> Route(Guid eventId, TEvent eventPayload); 
    }
}
