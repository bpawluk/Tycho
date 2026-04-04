using System;
using System.Collections.Generic;
using Tycho.Events.Handling;
using Tycho.Events.Routing;
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
            HandlerId = Handler is IIdentifiableEventHandler identifiableHandler
                ? identifiableHandler.Identity
                : EventHandlerIdentity.Create<TEventHandler, TEvent>();
        }

        public IReadOnlyCollection<RoutedEvent> Route(Guid eventId, TEvent eventPayload)
        {
            return new[] { new RoutedEvent<TEvent>(eventId, HandlerId, eventPayload) };
        }
    }
}
