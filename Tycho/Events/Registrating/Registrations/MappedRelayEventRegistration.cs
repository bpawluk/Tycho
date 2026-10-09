using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;
using Tycho.Events.Routing;

namespace Tycho.Events.Registrating.Registrations
{
    internal abstract class MappedRelayEventRegistration<TEvent, TTargetEvent> : IEventRegistration<TEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        private readonly IEventRouter _externalEventRouter;
        private readonly Func<TEvent, TTargetEvent> _map;

        public MappedRelayEventRegistration(IEventRouter externalEventRouter, Func<TEvent, TTargetEvent> map)
        {
            _externalEventRouter = externalEventRouter;
            _map = map;
        }

        public async Task<IReadOnlyCollection<RoutedEvent>> RouteAsync(
            Guid publishId,
            TEvent eventPayload,
            CancellationToken cancellationToken)
        {
            IReadOnlyCollection<RoutedEvent> routedEvents = await _externalEventRouter.RouteAsync(publishId, _map(eventPayload), cancellationToken).ConfigureAwait(false);

            return routedEvents;
        }
    }
}
