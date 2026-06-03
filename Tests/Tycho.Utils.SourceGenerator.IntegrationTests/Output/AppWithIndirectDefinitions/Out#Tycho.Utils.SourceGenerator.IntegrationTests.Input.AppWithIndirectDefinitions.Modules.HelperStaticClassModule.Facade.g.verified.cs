//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.HelperStaticClassModule.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    public class HelperStaticClassModuleFacade : ModuleFacadeBase, IHelperStaticClassModule
    {
        public HelperStaticClassModuleFacade(IModule<HelperStaticClassModule> module) : base(module) { }
    }
}
