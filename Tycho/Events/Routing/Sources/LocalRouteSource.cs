using System;
using System.Collections.Generic;
using System.Linq;
using Tycho.Registry;

namespace Tycho.Events.Routing.Sources
{
    internal class LocalRouteSource<TEvent> : IRouteSource<TEvent>
        where TEvent : class, IEvent
    {
        private readonly IEventHandlerRegistry _handlerRegistry;

        public LocalRouteSource(IEventHandlerRegistry handlerRegistry)
        {
            _handlerRegistry = handlerRegistry;
        }

        public IReadOnlyCollection<RoutedEvent> Route(Guid eventId, TEvent eventPayload)
        {
            return _handlerRegistry
                .IdenitfyHandlers<TEvent>()
                .Select(handlerId => new RoutedEvent<TEvent>(eventId, handlerId, eventPayload))
                .ToList();
        }
    }
}
