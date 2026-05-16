using System.Collections.Generic;
using Tycho.Modules.Instance;

namespace Tycho.Identity.Modules
{
    internal interface IModuleProvider
    {
        IModule GetModule(ModuleIdentity moduleId);

        IReadOnlyCollection<IModule> GetAllModules();
    }
}
