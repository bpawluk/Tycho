using System;

namespace Tycho.Identity.Structure
{
    internal sealed class InstanceIdentity : Identity, IEquatable<InstanceIdentity>
    {
        private InstanceIdentity(DefinitionIdentity definitionIdentity) : this(definitionIdentity, null) { }

        private InstanceIdentity(DefinitionIdentity definitionIdentity, InstanceIdentity? parent) : base(Construct(definitionIdentity, parent)) { }

        public bool Equals(InstanceIdentity other)
        {
            return this == other;
        }

        public InstanceIdentity CreateChild(DefinitionIdentity definitionIdentity)
        {
            return new InstanceIdentity(definitionIdentity, this);
        }

        public static InstanceIdentity CreateRoot(DefinitionIdentity definitionIdentity)
        {
            return new InstanceIdentity(definitionIdentity);
        }

        private static string Construct(DefinitionIdentity definitionIdentity, InstanceIdentity? parent)
        {
            if (definitionIdentity == null) throw new ArgumentNullException(nameof(definitionIdentity));
            return parent == null ? definitionIdentity.Value : parent.Value + "/" + definitionIdentity.Value;
        }
    }
}
