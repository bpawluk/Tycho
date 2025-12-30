using System.Collections.Generic;
using Tycho.Structure;

namespace Tycho.Registry
{
    internal interface IModuleRegistry
    {
        void RegisterModule(IModule module);

        IModule GetModule(ModuleIdentity moduleId);

        IReadOnlyCollection<IModule> GetAllModules();
    }
}
