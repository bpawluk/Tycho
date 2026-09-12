using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Steps;

namespace Tycho.Events.Delivery.Strategies
{
    internal class FinalRouteDelivery : IDeliveryStrategy
    {
        private readonly IInboxWriter _inbox;

        public FinalRouteDelivery(IInboxWriter inbox)
        {
            _inbox = inbox;
        }

        public bool CanDeliver(SerializedRoutedEvent routedEvent)
        {
            return routedEvent.Route.TryPeek(out IRouteStep? routeStep) && routeStep is FinalRouteStep;
        }

        public async Task DeliverAsync(SerializedRoutedEvent routedEvent, CancellationToken cancellationToken)
        {
            if (!routedEvent.Route.TryPop(out IRouteStep? routeStep) || !(routeStep is FinalRouteStep))
            {
                throw new InvalidOperationException($"Invalid route in {GetType().Name}.");
            }
            await _inbox.Write(routedEvent, cancellationToken);
        }
    }
}
