using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Model;
using Tycho.Events.Registrating.Registrations;
using Tycho.Structure;

namespace Tycho.Events.Broker
{
    internal class ScopedEventBroker : IEventBroker
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ControlPlane _controlPlane;

        public ScopedEventBroker(IServiceProvider serviceProvider, ControlPlane controlPlane)
        {
            _serviceProvider = serviceProvider;
            _controlPlane = controlPlane;
        }

        public async Task<IReadOnlyCollection<RoutedEvent>> RouteAsync<TEvent>(
            Guid publishId,
            TEvent eventPayload,
            CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            IEnumerable<IEventRegistration<TEvent>> registrations = _serviceProvider.GetServices<IEventRegistration<TEvent>>();
            var routedEvents = new List<RoutedEvent>();

            foreach (IEventRegistration<TEvent> registration in registrations)
            {
                IReadOnlyCollection<RoutedEvent> registrationEvents = await registration
                    .RouteAsync(publishId, eventPayload, cancellationToken)
                    .ConfigureAwait(false);
                routedEvents.AddRange(registrationEvents);
            }

            return routedEvents;
        }

        public async Task DeliverAsync(SerializedRoutedEvent routedEvent, CancellationToken cancellationToken)
        {
            await _controlPlane
                .GetModule(routedEvent.DestinationId)
                .Endpoint
                .AcceptAsync(routedEvent, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
