using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Routes;
using Tycho.Identity.Modules;

namespace Tycho.Events.Delivery.Strategies
{
    internal class DownStreamRouteDelivery : IDeliveryStrategy
    {
        private readonly IModuleProvider _moduleRegistry;

        public DownStreamRouteDelivery(IModuleProvider moduleRegistry)
        {
            _moduleRegistry = moduleRegistry;
        }

        public bool CanDeliver<TEvent>(RoutedEvent<TEvent> routedEvent)
            where TEvent : class, IEvent
        {
            return routedEvent.Route.TryPeek(out var routeStep) && routeStep is DownStreamRouteStep;
        }

        public async Task DeliverAsync<TEvent>(RoutedEvent<TEvent> routedEvent, CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            if (!routedEvent.Route.TryPop(out var routeStep) || !(routeStep is DownStreamRouteStep downStreamRouteStep))
            {
                throw new InvalidOperationException($"Invalid route in {GetType().Name}");
            }
            var submodule = _moduleRegistry.GetModule(downStreamRouteStep.Destination);
            await submodule.EventBroker.DeliverAsync(routedEvent, cancellationToken);
        }
    }
}
