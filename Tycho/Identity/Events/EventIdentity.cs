using System;
using Tycho.Events;

namespace Tycho.Identity.Events
{
    internal sealed class EventIdentity : TypeIdentity, IEquatable<EventIdentity>
    {
        private EventIdentity() { }

        private EventIdentity(string typeId) : base(typeId) { }

        private EventIdentity(Type eventType) : base(eventType) { }

        public bool Equals(EventIdentity? other)
        {
            return this == other;
        }

        public static EventIdentity Create<TEvent>() where TEvent : IEvent
        {
            return new EventIdentity(typeof(TEvent));
        }

        public static EventIdentity Parse(string identity)
        {
            return new EventIdentity(identity);
        }
    }
}
