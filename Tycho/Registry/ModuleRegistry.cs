using System;
using System.Collections.Generic;
using Tycho.Structure;

namespace Tycho.Registry
{
    internal class ModuleRegistry : IModuleRegistry
    {
        private readonly Dictionary<ModuleIdentity, IModuleInstance> _modules;

        public ModuleRegistry()
        {
            _modules = new Dictionary<ModuleIdentity, IModuleInstance>();
        }

        public void RegisterModule(IModuleInstance module)
        {
            var moduleType = module.Internals.Owner;
            var moduleIdentity = new ModuleIdentity(moduleType);
            _modules[moduleIdentity] = module;
        }

        public IModuleInstance GetModule(ModuleIdentity moduleId)
        {
            if (_modules.TryGetValue(moduleId, out var module))
            {
                return module;
            }
            throw new InvalidOperationException($"Module with identity '{moduleId}' not found.");
        }

        public IReadOnlyCollection<IModuleInstance> GetAllModules()
        {
            return _modules.Values;
        }
    }
}
