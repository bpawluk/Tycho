//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithSubmodules.Modules.ModuleA.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithSubmodules.Modules
{
    public class ModuleAFacade : ModuleFacadeBase, IModuleA
    {
        public ModuleAFacade(IModule<ModuleA> module) : base(module) { }
    }
}
