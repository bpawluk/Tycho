using System;

namespace Tycho.Identity.Events
{
    internal class EventHandlerIdentity : IEquatable<EventHandlerIdentity>
    {
        private const char _separator = '-';

        public string HandlerId { get; set; } = string.Empty;

        public string EventId { get; set; } = string.Empty;

        private EventHandlerIdentity(string handlerId, string eventId)
        {
            HandlerId = handlerId;
            EventId = eventId;
        }

        private EventHandlerIdentity(Type handlerType, Type eventType)
        {
            HandlerId = TypeIdentifier.GetId(handlerType);
            EventId = TypeIdentifier.GetId(eventType);
        }

        public static EventHandlerIdentity Create<THandler, TEvent>()
        {
            return new EventHandlerIdentity(typeof(THandler), typeof(TEvent));
        }

        public bool Equals(EventHandlerIdentity? other)
        {
            return this == other;
        }

        public override bool Equals(object? obj)
        {
            return this == obj as EventHandlerIdentity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(HandlerId, EventId);
        }

        public override string ToString()
        {
            return $"{HandlerId}{_separator}{EventId}";
        }

        public static EventHandlerIdentity Parse(string identity)
        {
            var parts = identity.Split(_separator);
            if (parts.Length != 2)
            {
                throw new FormatException($"Invalid {nameof(EventHandlerIdentity)} format: {identity}");
            }
            return new EventHandlerIdentity(parts[0], parts[1]);
        }

        public static bool operator !=(EventHandlerIdentity? left, EventHandlerIdentity? right)
        {
            return !(left == right);
        }

        public static bool operator ==(EventHandlerIdentity? left, EventHandlerIdentity? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return string.Equals(left.HandlerId, right.HandlerId, StringComparison.InvariantCulture) &&
                   string.Equals(left.EventId, right.EventId, StringComparison.InvariantCulture);
        }
    }
}
