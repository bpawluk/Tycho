using System;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.Events.Registrating.Registrations;
using Tycho.Structure;

namespace Tycho.Identity.Events
{
    internal class EventHandlerProvider : IEventHandlerProvider
    {
        private readonly Internals _internals;

        public EventHandlerProvider(Internals internals)
        {
            _internals = internals;
        }

        public IEventHandler<TEvent> GetHandler<TEvent>(EventHandlerIdentity handlerId) where TEvent : class, IEvent
        {
            foreach (var registration in _internals.GetServices<IFinalEventRegistration<TEvent>>())
            {
                if (registration.HandlerId == handlerId)
                {
                    return registration.Handler;
                }
            }
            throw new ArgumentException($"Event handler with identity '{handlerId}' is not registered for '{typeof(TEvent).Name}' event.", nameof(handlerId));
        }
    }
}
