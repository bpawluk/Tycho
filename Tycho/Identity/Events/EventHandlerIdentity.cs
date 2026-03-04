using System;

namespace Tycho.Identity.Events
{
    internal class EventHandlerIdentity : IEquatable<EventHandlerIdentity>
    {
        public string EventId { get; set; } = string.Empty;

        public string HandlerId { get; set; } = string.Empty;

        public EventHandlerIdentity(string eventId, string handlerId)
        {
            EventId = eventId;
            HandlerId = handlerId;
        }

        public EventHandlerIdentity(Type eventType, Type handlerType)
        {
            EventId = TypeIdentifier.GetId(eventType);
            HandlerId = TypeIdentifier.GetId(handlerType);
        }

        public bool MatchesEvent(Type eventType)
        {
            return EventId == TypeIdentifier.GetId(eventType);
        }

        public bool MatchesEvent<TEvent>()
        {
            return EventId == TypeIdentifier.GetId<TEvent>();
        }

        public bool MatchesHandler(Type handlerType)
        {
            return HandlerId == TypeIdentifier.GetId(handlerType);
        }

        public bool MatchesHandler<THandler>()
        {
            return HandlerId == TypeIdentifier.GetId<THandler>();
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
            return HashCode.Combine(EventId, HandlerId);
        }

        public override string ToString()
        {
            return $"{EventId}-{HandlerId}";
        }

        public static EventHandlerIdentity FromString(string identity)
        {
            var parts = identity.Split('-');
            if (parts.Length != 2)
            {
                throw new ArgumentException(
                    $"Invalid format of identity string {identity}",
                    nameof(identity));
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

            return string.Equals(left.EventId, right.EventId, StringComparison.InvariantCulture) &&
                   string.Equals(left.HandlerId, right.HandlerId, StringComparison.InvariantCulture);
        }
    }
}
