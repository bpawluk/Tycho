using System;

namespace Tycho.Identity
{
    internal abstract class TypeIdentity : Identity, IEquatable<TypeIdentity>
    {
        protected TypeIdentity(string typeId) : base(typeId)
        {
        }

        protected TypeIdentity(Type type) : base(TypeIdentifier.GetId(type))
        {
        }

        public bool Equals(TypeIdentity? other)
        {
            return this == other;
        }
    }
}
