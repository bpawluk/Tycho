using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Delivery;
using Tycho.Events.Model;
using Tycho.Events.Registrating.Registrations;

namespace Tycho.Events.Broker
{
    internal class ScopedEventBroker : IEventBroker
    {
        private readonly IServiceProvider _serviceProvider;

        public ScopedEventBroker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IReadOnlyCollection<RoutedEvent> Route<TEvent>(Guid eventId, TEvent eventPayload) 
            where TEvent : class, IEvent
        {
            var registrations = _serviceProvider.GetServices<IEventRegistration<TEvent>>();
            return registrations.SelectMany(registration => registration.Route(eventId, eventPayload)).ToArray();
        }

        public async Task DeliverAsync(SerializedRoutedEvent routedEvent, CancellationToken cancellationToken)
        {
            var deliveryStrategies = _serviceProvider.GetServices<IDeliveryStrategy>();

            var deliveryStrategy = deliveryStrategies.SingleOrDefault(s => s.CanDeliver(routedEvent));
            if (deliveryStrategy is null)
            {
                throw new InvalidOperationException($"No delivery strategy found for event with ID {routedEvent.EventId}.");
            }

            await deliveryStrategy.DeliverAsync(routedEvent, cancellationToken);
        }
    }
}
