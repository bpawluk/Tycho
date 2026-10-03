using System;

namespace Tycho.Identity.Structure
{
    internal sealed class DefinitionIdentity : TypeIdentity, IEquatable<DefinitionIdentity>
    {
        private DefinitionIdentity(string typeId) : base(typeId) { }

        private DefinitionIdentity(Type definitionType) : base(definitionType) { }

        public bool Equals(DefinitionIdentity? other)
        {
            return this == other;
        }

        public static DefinitionIdentity Create<TDefinition>()
        {
            return new DefinitionIdentity(typeof(TDefinition));
        }

        public static DefinitionIdentity Create(Type definitionType)
        {
            if (definitionType == null) throw new ArgumentNullException(nameof(definitionType));
            return new DefinitionIdentity(definitionType);
        }

        public static DefinitionIdentity Parse(string identity)
        {
            return new DefinitionIdentity(identity);
        }
    }
}
