using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing.Payload;
using Tycho.Events.Routing.Routes;
using Tycho.Structure.Internal;

namespace Tycho.Events.Routing.Delivery
{
    internal class DownStreamRouteDelivery : IDeliveryStrategy
    {
        private readonly ISubmoduleProvider _submoduleProvider;

        public DownStreamRouteDelivery(ISubmoduleProvider submoduleProvider)
        {
            _submoduleProvider = submoduleProvider;
        }

        public async Task DeliverAsync(IRoutedEvent routedEvent, CancellationToken cancellationToken)
        {
            if (!routedEvent.Route.TryPop(out var routeStep) || !(routeStep is DownStreamRouteStep downStreamRouteStep))
            {
                throw new InvalidOperationException($"Invalid route in {GetType().Name}");
            }

            var submodule = _submoduleProvider.GetSubmodule(downStreamRouteStep.Destination);
            await submodule.EventRouter.DeliverAsync(routedEvent, cancellationToken);
        }
    }
}
