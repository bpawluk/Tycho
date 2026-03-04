using System;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.Events.Registrating;
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

        public IEventHandler<TEvent> GetHandler<TEvent>(EventHandlerIdentity handlerId)
            where TEvent : class, IEvent
        {
            foreach (var registrations in _internals.GetServices<IHandlerRegistration<TEvent>>())
            {
                if (registrations.Identity == handlerId)
                {
                    return registrations.Handler;
                }
            }
            throw new ArgumentException($"Event handler with identity '{handlerId}' not found.", nameof(handlerId));
        }
    }
}
