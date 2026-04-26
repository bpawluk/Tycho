using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;
using Tycho.Events.Routing.Steps;
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

        public bool CanDeliver(SerializedRoutedEvent routedEvent)
        {
            return routedEvent.Route.TryPeek(out var routeStep) && routeStep is UpStreamRouteStep;
        }

        public async Task DeliverAsync(SerializedRoutedEvent routedEvent, CancellationToken cancellationToken)
        {
            if (!routedEvent.Route.TryPop(out var routeStep) || !(routeStep is UpStreamRouteStep))
            {
                throw new InvalidOperationException($"Invalid route in {GetType().Name}.");
            }
            await _parent.EventBroker.DeliverAsync(routedEvent, cancellationToken);
        }
    }
}
