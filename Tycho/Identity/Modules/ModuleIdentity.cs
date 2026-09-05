using System;
using Tycho.Modules;

namespace Tycho.Identity.Modules
{
    internal sealed class ModuleIdentity : TypeIdentity, IEquatable<ModuleIdentity>
    {
        private ModuleIdentity() { }

        private ModuleIdentity(string typeId) : base(typeId) { }

        private ModuleIdentity(Type moduleType) : base(moduleType) { }

        public bool Equals(ModuleIdentity? other)
        {
            return this == other;
        }

        public static ModuleIdentity Create<TModule>() where TModule : TychoModule
        {
            return new ModuleIdentity(typeof(TModule));
        }

        public static ModuleIdentity Parse(string identity)
        {
            return new ModuleIdentity(identity);
        }
    }
}
