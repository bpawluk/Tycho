//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.LocalStaticHelperModule.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    public class LocalStaticHelperModuleFacade : ModuleFacadeBase, ILocalStaticHelperModule
    {
        public LocalStaticHelperModuleFacade(IModule<LocalStaticHelperModule> module) : base(module) { }
    }
}
