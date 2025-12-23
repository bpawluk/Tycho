using System;
using Tycho.Utils;

namespace Tycho.Identities
{
    internal class ModuleIdentity : IEquatable<ModuleIdentity>
    {
        public string ModuleId { get; set; } = string.Empty;

        public ModuleIdentity(string eventId)
        {
            ModuleId = eventId;
        }

        public ModuleIdentity(Type eventType)
        {
            ModuleId = TypeIdentifier.GetId(eventType);
        }

        public bool MatchesModule(Type eventType)
        {
            return ModuleId == TypeIdentifier.GetId(eventType);
        }

        public bool MatchesModule<TModule>()
        {
            return ModuleId == TypeIdentifier.GetId<TModule>();
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

        public static ModuleIdentity FromString(string identity)
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
