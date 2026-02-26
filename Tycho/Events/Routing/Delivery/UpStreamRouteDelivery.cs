using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing.Payload;
using Tycho.Events.Routing.Routes;
using Tycho.Structure.External;

namespace Tycho.Events.Routing.Delivery
{
    internal class UpStreamRouteDelivery : IDeliveryStrategy
    {
        private readonly IParentReference _parent;

        public UpStreamRouteDelivery(IParentReference parent)
        {
            _parent = parent;
        }

        public async Task DeliverAsync(IRoutedEvent routedEvent, CancellationToken cancellationToken)
        {
            if (!routedEvent.Route.TryPop(out var routeStep) || !(routeStep is UpStreamRouteStep))
            {
                throw new InvalidOperationException($"Invalid route in {GetType().Name}");
            }

            await _parent.EventRouter.DeliverAsync(routedEvent, cancellationToken);
        }
    }
}
