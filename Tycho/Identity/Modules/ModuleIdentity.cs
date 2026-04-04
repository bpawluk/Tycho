using System;

namespace Tycho.Identity.Modules
{
    internal class ModuleIdentity : IEquatable<ModuleIdentity>
    {
        public string ModuleId { get; set; } = string.Empty;

        private ModuleIdentity(string moduleId)
        {
            ModuleId = moduleId;
        }

        private ModuleIdentity(Type moduleType)
        {
            ModuleId = TypeIdentifier.GetId(moduleType);
        }

        public static ModuleIdentity Create<TModule>()
        {
            return new ModuleIdentity(typeof(TModule));
        }

        public bool Equals(ModuleIdentity? other)
        {
            return this == other;
        }

        public override bool Equals(object? obj)
        {
            return this == obj as ModuleIdentity;
        }

        public override int GetHashCode()
        {
            return ModuleId.GetHashCode(StringComparison.InvariantCulture);
        }

        public override string ToString()
        {
            return ModuleId;
        }

        public static ModuleIdentity Parse(string identity)
        {
            return new ModuleIdentity(identity);
        }

        public static bool operator !=(ModuleIdentity? left, ModuleIdentity? right)
        {
            return !(left == right);
        }

        public static bool operator ==(ModuleIdentity? left, ModuleIdentity? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return string.Equals(left.ModuleId, right.ModuleId, StringComparison.InvariantCulture);
        }
    }
}
