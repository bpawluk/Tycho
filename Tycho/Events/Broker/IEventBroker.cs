using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;

namespace Tycho.Events.Broker
{
    internal interface IEventBroker
    {
        Task<IReadOnlyCollection<RoutedEvent>> RouteAsync<TEvent>(
            Guid publishId,
            TEvent eventPayload,
            CancellationToken cancellationToken)
            where TEvent : class, IEvent;

        Task DeliverAsync(SerializedRoutedEvent routedEvent, CancellationToken cancellationToken);
    }
}
