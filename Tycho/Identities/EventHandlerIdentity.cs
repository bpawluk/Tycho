using System;
using Tycho.Utils;

namespace Tycho.Identities
{
    internal class EventHandlerIdentity : IEquatable<EventHandlerIdentity>
    {
        public string EventId { get; set; } = string.Empty;

        public string HandlerId { get; set; } = string.Empty;

        public string ModuleId { get; set; } = string.Empty;

        public EventHandlerIdentity(string eventId, string handlerId, string moduleId)
        {
            EventId = eventId;
            HandlerId = handlerId;
            ModuleId = moduleId;
        }

        public EventHandlerIdentity(Type eventType, Type handlerType, Type moduleType)
        {
            EventId = TypeIdentifier.GetId(eventType);
            HandlerId = TypeIdentifier.GetId(handlerType);
            ModuleId = TypeIdentifier.GetId(moduleType);
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

        public bool MatchesModule(Type moduleType)
        {
            return ModuleId == TypeIdentifier.GetId(moduleType);
        }

        public bool MatchesModule<TModule>()
        {
            return ModuleId == TypeIdentifier.GetId<TModule>();
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
            return HashCode.Combine(EventId, HandlerId, ModuleId);
        }

        public override string ToString()
        {
            return $"{EventId}-{HandlerId}-{ModuleId}";
        }

        public static EventHandlerIdentity FromString(string identity)
        {
            var parts = identity.Split('-');
            if (parts.Length != 3)
            {
                throw new ArgumentException(
                    $"Invalid format of identity string {identity}",
                    nameof(identity));
            }
            return new EventHandlerIdentity(parts[0], parts[1], parts[2]);
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
                   string.Equals(left.HandlerId, right.HandlerId, StringComparison.InvariantCulture) &&
                   string.Equals(left.ModuleId, right.ModuleId, StringComparison.InvariantCulture);
        }
    }
}
