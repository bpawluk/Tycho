using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Routing
{
    internal interface IEventRouter
    {
        IReadOnlyCollection<RoutedEvent> Route<TEvent>(Guid eventId, TEvent eventPayload) 
            where TEvent : class, IEvent;

        Task DeliverAsync<TEvent>(RoutedEvent<TEvent> routedEvent, CancellationToken cancellationToken)
            where TEvent : class, IEvent;
    }
}
