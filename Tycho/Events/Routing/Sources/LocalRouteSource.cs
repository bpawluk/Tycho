using System;
using System.Collections.Generic;
using Tycho.Events.Registrating;
using Tycho.Identity.Events;

namespace Tycho.Events.Routing.Sources
{
    internal class LocalRouteSource<TEvent, TEventHandler> : IRouteSource<TEvent>, IHandlerRegistration<TEvent>
        where TEvent : class, IEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        public EventHandlerIdentity Identity { get; }

        public IEventHandler<TEvent> Handler { get; }

        public LocalRouteSource(TEventHandler handler)
        {
            Handler = handler;
            Identity = Handler is IEventHandlerIdentity identifiableHandler
                ? identifiableHandler.Identity
                : EventHandlerIdentity.Create<TEvent, TEventHandler>();
        }

        public IReadOnlyCollection<RoutedEvent> Route(Guid eventId, TEvent eventPayload)
        {
            return new[] { new RoutedEvent<TEvent>(eventId, Identity, eventPayload) };
        }
    }
}
