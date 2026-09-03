//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules.HelperClassModule.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules
{
    internal class HelperClassModuleFacade : ModuleFacadeBase, IHelperClassModule
    {
        public HelperClassModuleFacade(IModule<HelperClassModule> module) : base(module) { }
    }
}
