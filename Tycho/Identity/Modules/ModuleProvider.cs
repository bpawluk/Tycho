using System;
using System.Collections.Generic;
using System.Linq;
using Tycho.Modules.Instance;

namespace Tycho.Identity.Modules
{
    internal class ModuleProvider : IModuleProvider
    {
        private readonly IReadOnlyCollection<IModule> _modules;

        public ModuleProvider(IEnumerable<IModule> modules)
        {
            _modules = modules.ToArray();
        }

        public IModule GetModule(ModuleIdentity moduleId)
        {
            foreach (var module in _modules)
            {
                if (module.Identity == moduleId)
                {
                    return module;
                }
            }
            throw new ArgumentException($"Module with identity '{moduleId}' is not defined.", nameof(moduleId));
        }

        public IReadOnlyCollection<IModule> GetAllModules() => _modules;
    }
}
