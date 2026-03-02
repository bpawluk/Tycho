using System;
using System.Collections.Generic;
using Tycho.Events;
using Tycho.Events.Handling;
using Tycho.Structure;

namespace Tycho.Registry
{
    internal class EventHandlerRegistry : IEventHandlerRegistry
    {
        private readonly Dictionary<Type, HashSet<EventHandlerIdentity>> _identities;
        private readonly Dictionary<EventHandlerIdentity, Func<IEventHandler>> _factories;
        private readonly Internals _internals;

        public EventHandlerRegistry(Internals internals)
        {
            _internals = internals;
            _identities = new Dictionary<Type, HashSet<EventHandlerIdentity>>();
            _factories = new Dictionary<EventHandlerIdentity, Func<IEventHandler>>();
        }

        public void RegisterHandler<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>
        {
            var eventType = typeof(TEvent);
            var eventHandlerId = new EventHandlerIdentity(typeof(TEvent), typeof(THandler));

            if (!_identities.ContainsKey(eventType))
            {
                _identities[eventType] = new HashSet<EventHandlerIdentity>();
            }
            _identities[eventType].Add(eventHandlerId);

            _factories[eventHandlerId] = () => new ScopedEventHandler<TEvent, THandler>(_internals);
        }

        public IReadOnlyCollection<EventHandlerIdentity> IdenitfyHandlers<TEvent>()
            where TEvent : class, IEvent
        {
            if (_identities.TryGetValue(typeof(TEvent), out var identities))
            {
                return identities;
            }
            return Array.Empty<EventHandlerIdentity>();
        }

        public IEventHandler GetHandler(EventHandlerIdentity eventHandlerId)
        {
            if (_factories.TryGetValue(eventHandlerId, out var factory))
            {
                return factory();
            }
            throw new InvalidOperationException($"Handler with identity '{eventHandlerId}' not found.");
        }
    }
}
