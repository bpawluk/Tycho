using System;
using System.Collections.Generic;
using Tycho.Modules.Instance;

namespace Tycho.Registry
{
    internal class ModuleRegistry : IModuleRegistry
    {
        private readonly Dictionary<ModuleIdentity, IModule> _modules;

        public ModuleRegistry()
        {
            _modules = new Dictionary<ModuleIdentity, IModule>();
        }

        public void RegisterModule(IModule module)
        {
            var moduleType = module.Internals.Owner;
            var moduleIdentity = new ModuleIdentity(moduleType);
            _modules[moduleIdentity] = module;
        }

        public IModule GetModule(ModuleIdentity moduleId)
        {
            if (_modules.TryGetValue(moduleId, out var module))
            {
                return module;
            }
            throw new InvalidOperationException($"Module with identity '{moduleId}' not found.");
        }

        public IReadOnlyCollection<IModule> GetAllModules()
        {
            return _modules.Values;
        }
    }
}
