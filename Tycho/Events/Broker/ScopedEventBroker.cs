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

        public IReadOnlyCollection<RoutedEvent> Route<TEvent>(Guid publishId, TEvent eventPayload)
            where TEvent : class, IEvent
        {
            IEnumerable<IEventRegistration<TEvent>> registrations = _serviceProvider.GetServices<IEventRegistration<TEvent>>();
            return registrations.SelectMany(registration => registration.Route(publishId, eventPayload)).ToArray();
        }

        public async Task DeliverAsync(SerializedRoutedEvent routedEvent, CancellationToken cancellationToken)
        {
            IEnumerable<IDeliveryStrategy> deliveryStrategies = _serviceProvider.GetServices<IDeliveryStrategy>();

            IDeliveryStrategy? deliveryStrategy = deliveryStrategies.SingleOrDefault(s => s.CanDeliver(routedEvent)) ?? throw new InvalidOperationException($"No delivery strategy found for event with ID {routedEvent.EventId}.");
            await deliveryStrategy.DeliverAsync(routedEvent, cancellationToken);
        }
    }
}
