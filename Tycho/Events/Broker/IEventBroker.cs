using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;
using Tycho.Events.Serialization;

namespace Tycho.Events.Broker
{
    internal interface IEventBroker
    {
        IReadOnlyCollection<RoutedEvent> Route<TEvent>(Guid eventId, TEvent eventPayload) 
            where TEvent : class, IEvent;

        Task DeliverAsync<TEvent>(RoutedEvent<TEvent> routedEvent, CancellationToken cancellationToken)
            where TEvent : class, IEvent;
    }
}
