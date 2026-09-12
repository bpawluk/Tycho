using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Steps;
using Tycho.Identity.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Events.Delivery.Strategies
{
    internal class DownStreamRouteDelivery : IDeliveryStrategy
    {
        private readonly IModuleProvider _moduleProvider;

        public DownStreamRouteDelivery(IModuleProvider moduleProvider)
        {
            _moduleProvider = moduleProvider;
        }

        public bool CanDeliver(SerializedRoutedEvent routedEvent)
        {
            return routedEvent.Route.TryPeek(out IRouteStep? routeStep) && routeStep is DownStreamRouteStep;
        }

        public async Task DeliverAsync(SerializedRoutedEvent routedEvent, CancellationToken cancellationToken)
        {
            if (!routedEvent.Route.TryPop(out IRouteStep? routeStep) || !(routeStep is DownStreamRouteStep downStreamRouteStep))
            {
                throw new InvalidOperationException($"Invalid route in {GetType().Name}.");
            }

            IModule? submodule = _moduleProvider.GetModule(downStreamRouteStep.Destination) ?? throw new InvalidOperationException($"Module specified in {routeStep} route step is missing.");
            await submodule.EventBroker.DeliverAsync(routedEvent, cancellationToken);
        }
    }
}
