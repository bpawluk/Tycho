using System;
using Tycho.Events;

namespace Tycho.Identity.Events
{
    internal sealed class EventHandlerIdentity : TypeIdentity, IEquatable<EventHandlerIdentity>
    {
        private EventHandlerIdentity() { }

        private EventHandlerIdentity(string typeId) : base(typeId) { }

        private EventHandlerIdentity(Type eventHandlerType) : base(eventHandlerType) { }

        public bool Equals(EventHandlerIdentity? other)
        {
            return this == other;
        }

        public static EventHandlerIdentity Create<TEventHandler>() where TEventHandler : IEventHandler
        {
            return new EventHandlerIdentity(typeof(TEventHandler));
        }

        public static EventHandlerIdentity Parse(string identity)
        {
            return new EventHandlerIdentity(identity);
        }
    }
}
