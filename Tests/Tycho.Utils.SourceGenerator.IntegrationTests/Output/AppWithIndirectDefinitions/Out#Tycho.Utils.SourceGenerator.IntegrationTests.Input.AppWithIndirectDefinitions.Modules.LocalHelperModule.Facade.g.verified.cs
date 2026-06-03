//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.LocalHelperModule.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    public class LocalHelperModuleFacade : ModuleFacadeBase, ILocalHelperModule
    {
        public LocalHelperModuleFacade(IModule<LocalHelperModule> module) : base(module) { }
    }
}
