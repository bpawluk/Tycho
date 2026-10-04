using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;
using Tycho.Identity.Events;
using Tycho.Structure;

namespace Tycho.Events.Registrating.Registrations
{
    internal class FinalEventRegistration<TEvent, TEventHandler> : IFinalEventRegistration<TEvent>
        where TEvent : class, IEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        private readonly Internals _internals;

        public IEventHandler<TEvent> Handler { get; }

        public EventHandlerIdentity HandlerId { get; }

        public FinalEventRegistration(TEventHandler handler, Internals internals)
        {
            Handler = handler;
            _internals = internals;
            HandlerId = EventHandlerIdentity.Create<TEventHandler>();
        }

        public Task<IReadOnlyCollection<RoutedEvent>> RouteAsync(
            Guid publishId,
            TEvent eventPayload,
            CancellationToken cancellationToken)
        {
            var eventId = EventIdentity.Create<TEvent>();
            IReadOnlyCollection<RoutedEvent> routedEvents = new[]
            {
                new RoutedEvent<TEvent>(
                    Guid.NewGuid(),
                    publishId,
                    eventId,
                    HandlerId,
                    _internals.OwnerId,
                    eventPayload)
            };
            return Task.FromResult(routedEvents);
        }
    }
}
