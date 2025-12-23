using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Inbox;
using Tycho.Events.Routing.Payload;
using Tycho.Events.Routing.Routes;

namespace Tycho.Events.Routing.Delivery
{
    internal class FinalRouteDelivery : IDeliveryStrategy
    {
        private readonly IInboxWriter _inbox;

        public FinalRouteDelivery(IInboxWriter inbox)
        {
            _inbox = inbox;
        }

        public async Task DeliverAsync(IRoutedEvent routedEvent, CancellationToken cancellationToken)
        {
            if (!routedEvent.Route.TryPop(out var routeStep) || !(routeStep is FinalRouteStep finalRouteStep))
            {
                throw new InvalidOperationException($"Invalid route in {GetType().Name}");
            }

            var inboxEntry = new InboxEntry(
                routedEvent.Id,
                routedEvent.Payload,
                finalRouteStep.HandlerId);

            await _inbox.Write(inboxEntry, cancellationToken);
        }
    }
}
