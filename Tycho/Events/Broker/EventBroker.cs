using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Model;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Events.Broker
{
    internal class EventBroker : IEventBroker
    {
        private readonly Internals _internals;

        public EventBroker(Internals internals)
        {
            _internals = internals;
        }

        [EntryPoint]
        public IReadOnlyCollection<RoutedEvent> Route<TEvent>(Guid eventId, TEvent eventPayload)
            where TEvent : class, IEvent
        {
            using IServiceScope scope = _internals.CreateScope();
            IEventBroker scopedBroker = scope.ServiceProvider.GetRequiredService<IEventBroker>();
            return scopedBroker.Route(eventId, eventPayload);
        }

        [EntryPoint]
        public async Task DeliverAsync(SerializedRoutedEvent routedEvent, CancellationToken cancellationToken)
        {
            await using AsyncServiceScope scope = _internals.CreateAsyncScope();
            IEventBroker scopedBroker = scope.ServiceProvider.GetRequiredService<IEventBroker>();
            await scopedBroker.DeliverAsync(routedEvent, cancellationToken);
        }
    }
}
