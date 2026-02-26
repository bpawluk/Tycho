using System.Collections.Generic;
using Tycho.Modules.Instance;

namespace Tycho.Registry
{
    internal interface IModuleRegistry
    {
        void RegisterModule(IModule module);

        IModule GetModule(ModuleIdentity moduleId);

        IReadOnlyCollection<IModule> GetAllModules();
    }
}
