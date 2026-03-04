using System;
using System.Collections.Generic;
using Tycho.Events.Broker;
using Tycho.Events.Routing.Routes;

namespace Tycho.Events.Routing.Sources
{
    internal abstract class MappedExternalRouteSource<TEvent, TTargetEvent> : IRouteSource<TEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        private readonly IEventBroker _externalEventBroker;
        private readonly Func<TEvent, TTargetEvent> _map;

        public MappedExternalRouteSource(IEventBroker externalEventBroker, Func<TEvent, TTargetEvent> map)
        {
            _externalEventBroker = externalEventBroker;
            _map = map;
        }

        public IReadOnlyCollection<RoutedEvent> Route(Guid eventId, TEvent eventPayload)
        {
            var routeStep = GetRouteStep();
            var routedEvents = _externalEventBroker.Route(eventId, _map(eventPayload));

            foreach (var routedEvent in routedEvents)
            {
                routedEvent.Route.Push(routeStep);
            }

            return routedEvents;
        }

        protected abstract RouteStep GetRouteStep();
    }
}
