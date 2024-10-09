using System;

namespace TychoV2.Events.Routing
{
    internal class HandlerIdentity : IEquatable<HandlerIdentity>
    {
        public string SourceId { get; set; } = string.Empty;

        public string HandlerId { get; set; } = string.Empty;

        public HandlerIdentity(string sourceId, string handlerId)
        {
            SourceId = sourceId;
            HandlerId = handlerId;
        }

        public bool Equals(HandlerIdentity? other) => this == other;

        public override bool Equals(object? obj) => this == obj as HandlerIdentity;

        public override int GetHashCode() => HashCode.Combine(SourceId, HandlerId);

        public static bool operator !=(HandlerIdentity? left, HandlerIdentity? right) => !(left == right);

        public static bool operator ==(HandlerIdentity? left, HandlerIdentity? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return string.Equals(left.SourceId, right.SourceId, StringComparison.InvariantCulture) &&
                   string.Equals(left.HandlerId, right.HandlerId, StringComparison.InvariantCulture);
        }

        public override string ToString() => $"{SourceId}-{HandlerId}";
    }
}
