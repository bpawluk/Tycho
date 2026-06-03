//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.HelperClassModule.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    public class HelperClassModuleFacade : ModuleFacadeBase, IHelperClassModule
    {
        public HelperClassModuleFacade(IModule<HelperClassModule> module) : base(module) { }
    }
}
