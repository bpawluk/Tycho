using System.Collections.Generic;
using Tycho.Structure;

namespace Tycho.Registry
{
    internal interface IModuleRegistry
    {
        void RegisterModule(IModuleInstance module);

        IModuleInstance GetModule(ModuleIdentity moduleId);

        IReadOnlyCollection<IModuleInstance> GetAllModules();
    }
}
