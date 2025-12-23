using System;
using System.Collections.Generic;
using Tycho.Events.Routing.Payload;
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

        public IReadOnlyCollection<IRoutedEvent<IEvent>> GetRoutes(Guid eventId, TEvent eventPayload)
        {
            var routeStep = GetRouteStep();
            var mappedPayload = _map(eventPayload);
            var routedEvents = _externalEventRouter.FindRoutes(eventId, mappedPayload);

            foreach (var routedEvent in routedEvents)
            {
                routedEvent.Route.Push(routeStep);
            }

            return routedEvents;
        }

        protected abstract IRouteStep GetRouteStep();
    }
}
