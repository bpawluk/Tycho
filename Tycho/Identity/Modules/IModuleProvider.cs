using System.Collections.Generic;
using Tycho.Identity.Structure;
using Tycho.Modules.Instance;

namespace Tycho.Identity.Modules
{
    internal interface IModuleProvider
    {
        IModule GetModule(DefinitionIdentity moduleId);

        IReadOnlyCollection<IModule> GetAllModules();
    }
}
