//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithUpstreamContract.TestModule.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithUpstreamContract
{
    public class TestModuleFacade : ModuleFacadeBase, ITestModule
    {
        public TestModuleFacade(IModule<TestModule> module) : base(module) { }
    }
}
