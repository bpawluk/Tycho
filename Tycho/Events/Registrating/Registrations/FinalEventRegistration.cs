using System;
using System.Collections.Generic;
using Tycho.Events.Model;
using Tycho.Identity.Events;

namespace Tycho.Events.Registrating.Registrations
{
    internal class FinalEventRegistration<TEvent, TEventHandler> : IFinalEventRegistration<TEvent>
        where TEvent : class, IEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        public IEventHandler<TEvent> Handler { get; }

        public EventHandlerIdentity HandlerId { get; }

        public FinalEventRegistration(TEventHandler handler)
        {
            Handler = handler;
            HandlerId = EventHandlerIdentity.Create<TEventHandler>();
        }

        public IReadOnlyCollection<RoutedEvent> Route(Guid publishId, TEvent eventPayload)
        {
            var eventId = EventIdentity.Create<TEvent>();
            var route = Routing.Route.Create();
            return new[] { new RoutedEvent<TEvent>(Guid.NewGuid(), publishId, eventId, HandlerId, route, eventPayload) };
        }
    }
}
