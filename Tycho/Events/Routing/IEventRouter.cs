using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing.Payload;

namespace Tycho.Events.Routing
{
    internal interface IEventRouter
    {
        IReadOnlyCollection<IRoutedEvent<IEvent>> FindRoutes<TEvent>(Guid eventId, TEvent eventPayload) 
            where TEvent : class, IEvent;

        Task DeliverAsync(IRoutedEvent routedEvent, CancellationToken cancellationToken);
    }
}
