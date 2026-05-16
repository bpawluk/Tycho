using System;
using System.Collections.Generic;
using Tycho.Events.Broker;
using Tycho.Events.Model;
using Tycho.Events.Routing;

namespace Tycho.Events.Registrating.Registrations
{
    internal abstract class MappedRelayEventRegistration<TEvent, TTargetEvent> : IEventRegistration<TEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        private readonly IEventBroker _externalEventBroker;
        private readonly Func<TEvent, TTargetEvent> _map;

        public MappedRelayEventRegistration(IEventBroker externalEventBroker, Func<TEvent, TTargetEvent> map)
        {
            _externalEventBroker = externalEventBroker;
            _map = map;
        }

        public IReadOnlyCollection<RoutedEvent> Route(Guid eventId, TEvent eventPayload)
        {
            IRouteStep routeStep = GetRouteStep();
            IReadOnlyCollection<RoutedEvent> routedEvents = _externalEventBroker.Route(eventId, _map(eventPayload));

            foreach (RoutedEvent routedEvent in routedEvents)
            {
                routedEvent.Route.Push(routeStep);
            }

            return routedEvents;
        }

        protected abstract IRouteStep GetRouteStep();
    }
}
