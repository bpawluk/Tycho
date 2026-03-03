using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Routes;
using Tycho.Structure.Parent;

namespace Tycho.Events.Delivery.Strategies
{
    internal class UpStreamRouteDelivery : IDeliveryStrategy
    {
        private readonly IParentReference _parent;

        public UpStreamRouteDelivery(IParentReference parent)
        {
            _parent = parent;
        }

        public async Task DeliverAsync<TEvent>(RoutedEvent<TEvent> routedEvent, CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            if (!routedEvent.Route.TryPop(out var routeStep) || !(routeStep is UpStreamRouteStep))
            {
                throw new InvalidOperationException($"Invalid route in {GetType().Name}");
            }
            await _parent.EventRouter.DeliverAsync(routedEvent, cancellationToken);
        }
    }
}
