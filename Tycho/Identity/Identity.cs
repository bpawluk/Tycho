using System;

namespace Tycho.Identity
{
    internal abstract class Identity : IEquatable<Identity>
    {
        public string Value { get; }

        protected Identity(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public bool Equals(Identity? other) => this == other;

        public override bool Equals(object? obj) => obj is Identity other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode(StringComparison.InvariantCulture);

        public override string ToString() => Value;

        public static bool operator !=(Identity? left, Identity? right) => !(left == right);

        public static bool operator ==(Identity? left, Identity? right)
        {
            if (ReferenceEquals(left, right)) return true;

            if (left is null || right is null) return false;

            return string.Equals(left.Value, right.Value, StringComparison.InvariantCulture);
        }
    }
}
