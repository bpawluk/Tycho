using System;
using System.Collections.Generic;
using Tycho.Events.Routing.Routes;

namespace Tycho.Events.Routing.Sources
{
    internal abstract class ExternalRouteSource<TEvent> : IRouteSource<TEvent>
        where TEvent : class, IEvent
    {
        private readonly IEventRouter _externalEventRouter;

        public ExternalRouteSource(IEventRouter externalEventRouter)
        {
            _externalEventRouter = externalEventRouter;
        }

        public IReadOnlyCollection<RoutedEvent> Route(Guid eventId, TEvent eventPayload)
        {
            var routeStep = GetRouteStep();
            var routedEvents = _externalEventRouter.Route(eventId, eventPayload);

            foreach (var routedEvent in routedEvents)
            {
                routedEvent.Route.Push(routeStep);
            }   

            return routedEvents;
        }

        protected abstract RouteStep GetRouteStep();
    }
}
