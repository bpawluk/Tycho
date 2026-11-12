using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;
using Tycho.Events.Routing;

namespace Tycho.Events.Registrating.Registrations
{
    internal abstract class RelayEventRegistration<TEvent> : IEventRegistration<TEvent>
        where TEvent : class, IEvent
    {
        private readonly IEventRouter _externalEventRouter;

        public RelayEventRegistration(IEventRouter externalEventRouter)
        {
            _externalEventRouter = externalEventRouter;
        }

        public async Task<IReadOnlyCollection<RoutedEvent>> RouteAsync(
            Guid publishId,
            TEvent eventPayload,
            CancellationToken cancellationToken)
        {
            IReadOnlyCollection<RoutedEvent> routedEvents = await _externalEventRouter.RouteAsync(publishId, eventPayload, cancellationToken).ConfigureAwait(false);

            return routedEvents;
        }
    }
}
