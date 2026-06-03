using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;

namespace Tycho.Events.Broker
{
    internal interface IEventBroker
    {
        IReadOnlyCollection<RoutedEvent> Route<TEvent>(Guid publishId, TEvent eventPayload)
            where TEvent : class, IEvent;

        Task DeliverAsync(SerializedRoutedEvent routedEvent, CancellationToken cancellationToken);
    }
}
