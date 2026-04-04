using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Inbox;
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

        public bool CanDeliver<TEvent>(RoutedEvent<TEvent> routedEvent)
            where TEvent : class, IEvent
        {
            return routedEvent.Route.TryPeek(out var routeStep) && routeStep is FinalRouteStep;
        }

        public async Task DeliverAsync<TEvent>(RoutedEvent<TEvent> routedEvent, CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            if (!routedEvent.Route.TryPop(out var routeStep) || !(routeStep is FinalRouteStep))
            {
                throw new InvalidOperationException($"Invalid route in {GetType().Name}");
            }
            await _inbox.Write(routedEvent, cancellationToken);
        }
    }
}
