using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Routing.Payload;
using Tycho.Identities;
using Tycho.Structure.Internal;

namespace Tycho.Events.Routing.Sources
{
    internal class LocalRouteSource<TEvent> : IRouteSource<TEvent>
        where TEvent : class, IEvent
    {
        private readonly Internals _internals;

        public LocalRouteSource(Internals internals)
        {
            _internals = internals;
        }

        public IReadOnlyCollection<IRoutedEvent<IEvent>> GetRoutes(Guid eventId, TEvent eventPayload)
        {
            return _internals
                .GetServices<IEventHandler<TEvent>>()
                .Select(handler =>
                    new RoutedEvent<TEvent>(
                        eventId, 
                        eventPayload,
                        GetHandlerIdentity(handler)))
                .ToArray();
        }

        private EventHandlerIdentity GetHandlerIdentity(IEventHandler<TEvent> handler) =>
            new EventHandlerIdentity(
                handler.EventType,
                handler.HandlerType,
                _internals.Owner);
    }
}
