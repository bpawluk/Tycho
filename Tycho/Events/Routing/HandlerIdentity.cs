using System;
using Tycho.Utils;

namespace Tycho.Events.Routing
{
    internal class HandlerIdentity : IEquatable<HandlerIdentity>
    {
        public string EventId { get; set; } = string.Empty;

        public string HandlerId { get; set; } = string.Empty;

        public string ModuleId { get; set; } = string.Empty;

        public HandlerIdentity(string eventId, string handlerId, string moduleId)
        {
            EventId = eventId;
            HandlerId = handlerId;
            ModuleId = moduleId;
        }

        public HandlerIdentity(Type eventType, Type handlerType, Type moduleType)
        {
            EventId = TypeIdentifier.GetId(eventType);
            HandlerId = TypeIdentifier.GetId(handlerType);
            ModuleId = TypeIdentifier.GetId(moduleType);
        }

        public HandlerIdentity ForEvent(Type eventType)
        {
            return new HandlerIdentity(TypeIdentifier.GetId(eventType), HandlerId, ModuleId);
        }

        public bool Equals(HandlerIdentity? other)
        {
            return this == other;
        }

        public bool MatchesEvent(Type eventType)
        {
            return EventId == TypeIdentifier.GetId(eventType);
        }

        public bool MatchesHandler(Type handlerType)
        {
            return HandlerId == TypeIdentifier.GetId(handlerType);
        }

        public bool MatchesModule(Type moduleType)
        {
            return ModuleId == TypeIdentifier.GetId(moduleType);
        }

        public override bool Equals(object? obj)
        {
            return this == obj as HandlerIdentity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ModuleId, HandlerId);
        }

        public static bool operator !=(HandlerIdentity? left, HandlerIdentity? right)
        {
            return !(left == right);
        }

        public static bool operator ==(HandlerIdentity? left, HandlerIdentity? right)
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

        public override string ToString()
        {
            return $"{EventId}-{HandlerId}-{ModuleId}";
        }
    }
}