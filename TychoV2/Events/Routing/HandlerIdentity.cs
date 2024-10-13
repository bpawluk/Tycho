using System;
using TychoV2.Utils;

namespace TychoV2.Events.Routing
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

        public bool MatchesEvent(Type eventType) => EventId == TypeIdentifier.GetId(eventType);

        public bool MatchesHandler(Type handlerType) => HandlerId == TypeIdentifier.GetId(handlerType);

        public bool MatchesModule(Type moduleType) => ModuleId == TypeIdentifier.GetId(moduleType);

        public bool Equals(HandlerIdentity? other) => this == other;

        public override bool Equals(object? obj) => this == obj as HandlerIdentity;

        public override int GetHashCode() => HashCode.Combine(ModuleId, HandlerId);

        public static bool operator !=(HandlerIdentity? left, HandlerIdentity? right) => !(left == right);

        public static bool operator ==(HandlerIdentity? left, HandlerIdentity? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return string.Equals(left.EventId, right.EventId, StringComparison.InvariantCulture) &&
                   string.Equals(left.HandlerId, right.HandlerId, StringComparison.InvariantCulture) &&
                   string.Equals(left.ModuleId, right.ModuleId, StringComparison.InvariantCulture);
        }

        public override string ToString() => $"{EventId}-{HandlerId}-{ModuleId}";
    }
}
