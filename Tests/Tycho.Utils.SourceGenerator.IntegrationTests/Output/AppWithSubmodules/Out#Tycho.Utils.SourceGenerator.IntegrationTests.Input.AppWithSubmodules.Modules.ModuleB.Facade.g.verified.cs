//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.Modules.ModuleB.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.Modules
{
    public class ModuleBFacade : ModuleFacadeBase, IModuleB
    {
        public ModuleBFacade(IModule<ModuleB> module) : base(module) { }
    }
}
