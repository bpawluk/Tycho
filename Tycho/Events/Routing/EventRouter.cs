using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Routing.Delivery;
using Tycho.Events.Routing.Payload;
using Tycho.Events.Routing.Sources;
using Tycho.Structure.Internal;

namespace Tycho.Events.Routing
{
    internal class EventRouter : IEventRouter
    {
        private readonly Internals _internals;

        public EventRouter(Internals internals)
        {
            _internals = internals;
        }

        public IReadOnlyCollection<IRoutedEvent<IEvent>> FindRoutes<TEvent>(Guid eventId, TEvent eventPayload) 
            where TEvent : class, IEvent
        {
            var sources = _internals.GetServices<IRouteSource<TEvent>>();
            return sources.SelectMany(source => source.GetRoutes(eventId, eventPayload)).ToArray();
        }

        public async Task DeliverAsync(IRoutedEvent routedEvent, CancellationToken cancellationToken)
        {
            if (!routedEvent.Route.TryPeek(out var nextRouteStep))
            {
                throw new InvalidOperationException("No route steps available in the routed event.");
            }

            var deliveryStrategyProvider = _internals.GetRequiredService<IDeliveryStrategyProvider>();
            var deliveryStrategy = deliveryStrategyProvider.GetDeliveryStrategy(nextRouteStep);

            await deliveryStrategy.DeliverAsync(routedEvent, cancellationToken);
        }
    }
}
