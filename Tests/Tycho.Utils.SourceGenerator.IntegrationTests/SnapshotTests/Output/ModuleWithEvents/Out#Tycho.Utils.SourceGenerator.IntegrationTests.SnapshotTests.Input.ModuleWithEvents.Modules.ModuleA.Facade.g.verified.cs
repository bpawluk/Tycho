//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Modules.ModuleA.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Modules
{
    internal class ModuleAFacade : ModuleFacadeBase, IModuleA
    {
        public ModuleAFacade(IModule<ModuleA> module) : base(module) { }
    }
}
