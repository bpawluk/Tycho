using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules.Instance;
using Tycho.Structure;

namespace Tycho.Identity.Modules
{
    internal class ModuleProvider : IModuleProvider
    {
        private readonly Internals _internals;

        public ModuleProvider(Internals internals)
        {
            _internals = internals;
        }

        public IModule GetModule(ModuleIdentity moduleId)
        {
            foreach (var module in _internals.GetServices<IModule>())
            {
                if (module.Identity == moduleId)
                {
                    return module;
                }
            }
            throw new ArgumentException($"Module with identity '{moduleId}' is not defined.", nameof(moduleId));
        }

        public IReadOnlyCollection<IModule> GetAllModules() => _internals.GetServices<IModule>().ToArray();
    }
}
