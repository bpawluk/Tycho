using System;

namespace Tycho.Identity
{
    internal abstract class TypeIdentity : IEquatable<TypeIdentity>
    {
        public string TypeId { get; set; } = string.Empty;

        protected TypeIdentity()
        {
            throw new NotImplementedException();
        }

        protected TypeIdentity(string typeId)
        {
            TypeId = typeId;
        }

        protected TypeIdentity(Type type)
        {
            TypeId = TypeIdentifier.GetId(type);
        }

        public bool Equals(TypeIdentity? other)
        {
            return this == other;
        }

        public override bool Equals(object? obj)
        {
            return this == obj as TypeIdentity;
        }

        public override int GetHashCode()
        {
            return TypeId.GetHashCode(StringComparison.InvariantCulture);
        }

        public override string ToString()
        {
            return TypeId;
        }

        public static bool operator !=(TypeIdentity? left, TypeIdentity? right)
        {
            return !(left == right);
        }

        public static bool operator ==(TypeIdentity? left, TypeIdentity? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return string.Equals(left.TypeId, right.TypeId, StringComparison.InvariantCulture);
        }
    }
}
