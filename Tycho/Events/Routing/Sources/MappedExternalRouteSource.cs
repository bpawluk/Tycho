using System;
using System.Collections.Generic;
using Tycho.Events.Routing.Routes;

namespace Tycho.Events.Routing.Sources
{
    internal abstract class MappedExternalRouteSource<TEvent, TTargetEvent> : IRouteSource<TEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        private readonly IEventRouter _externalEventRouter;
        private readonly Func<TEvent, TTargetEvent> _map;

        public MappedExternalRouteSource(IEventRouter externalEventRouter, Func<TEvent, TTargetEvent> map)
        {
            _externalEventRouter = externalEventRouter;
            _map = map;
        }

        public IReadOnlyCollection<RoutedEvent> Route(Guid eventId, TEvent eventPayload)
        {
            var routeStep = GetRouteStep();
            var routedEvents = _externalEventRouter.Route(eventId, _map(eventPayload));

            foreach (var routedEvent in routedEvents)
            {
                routedEvent.Route.Push(routeStep);
            }

            return routedEvents;
        }

        protected abstract RouteStep GetRouteStep();
    }
}
