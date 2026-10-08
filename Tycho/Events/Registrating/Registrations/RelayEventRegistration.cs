using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Broker;
using Tycho.Events.Model;

namespace Tycho.Events.Registrating.Registrations
{
    internal abstract class RelayEventRegistration<TEvent> : IEventRegistration<TEvent>
        where TEvent : class, IEvent
    {
        private readonly IEventBroker _externalEventBroker;

        public RelayEventRegistration(IEventBroker externalEventBroker)
        {
            _externalEventBroker = externalEventBroker;
        }

        public async Task<IReadOnlyCollection<RoutedEvent>> RouteAsync(
            Guid publishId,
            TEvent eventPayload,
            CancellationToken cancellationToken)
        {
            IReadOnlyCollection<RoutedEvent> routedEvents = await _externalEventBroker.RouteAsync(publishId, eventPayload, cancellationToken).ConfigureAwait(false);

            return routedEvents;
        }
    }
}
