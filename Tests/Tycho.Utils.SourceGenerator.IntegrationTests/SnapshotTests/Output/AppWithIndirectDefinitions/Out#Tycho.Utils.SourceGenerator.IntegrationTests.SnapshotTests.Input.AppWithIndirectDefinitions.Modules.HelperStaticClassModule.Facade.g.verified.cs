//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules.HelperStaticClassModule.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules
{
    internal class HelperStaticClassModuleFacade : ModuleFacadeBase, IHelperStaticClassModule
    {
        public HelperStaticClassModuleFacade(IModule<HelperStaticClassModule> module) : base(module) { }
    }
}
