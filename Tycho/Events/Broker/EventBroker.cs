using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Delivery;
using Tycho.Events.Registrating.Registrations;
using Tycho.Events.Routing;
using Tycho.Structure;

namespace Tycho.Events.Broker
{
    internal class EventBroker : IEventBroker
    {
        private readonly Internals _internals;

        public EventBroker(Internals internals)
        {
            _internals = internals;
        }

        public IReadOnlyCollection<RoutedEvent> Route<TEvent>(Guid eventId, TEvent eventPayload) 
            where TEvent : class, IEvent
        {
            var registrations = _internals.GetServices<IEventRegistration<TEvent>>();
            return registrations.SelectMany(registration => registration.Route(eventId, eventPayload)).ToArray();
        }

        public async Task DeliverAsync<TEvent>(RoutedEvent<TEvent> routedEvent, CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            var deliveryStrategies = _internals.GetServices<IDeliveryStrategy>();
            var deliveryStrategy = deliveryStrategies.Single(s => s.CanDeliver(routedEvent));
            await deliveryStrategy.DeliverAsync(routedEvent, cancellationToken);
        }
    }
}
