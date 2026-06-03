using System;
using System.Collections.Generic;
using Tycho.Events.Broker;
using Tycho.Events.Model;
using Tycho.Events.Routing;

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

        public IReadOnlyCollection<RoutedEvent> Route(Guid publishId, TEvent eventPayload)
        {
            IRouteStep routeStep = GetRouteStep();
            IReadOnlyCollection<RoutedEvent> routedEvents = _externalEventBroker.Route(publishId, eventPayload);

            foreach (RoutedEvent routedEvent in routedEvents)
            {
                routedEvent.Route.Push(routeStep);
            }

            return routedEvents;
        }

        protected abstract IRouteStep GetRouteStep();
    }
}
