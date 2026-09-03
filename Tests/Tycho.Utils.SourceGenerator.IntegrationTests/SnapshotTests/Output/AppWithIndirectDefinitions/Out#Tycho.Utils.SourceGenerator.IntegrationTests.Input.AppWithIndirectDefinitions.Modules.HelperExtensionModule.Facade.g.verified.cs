//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.HelperExtensionModule.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    internal class HelperExtensionModuleFacade : ModuleFacadeBase, IHelperExtensionModule
    {
        public HelperExtensionModuleFacade(IModule<HelperExtensionModule> module) : base(module) { }
    }
}
