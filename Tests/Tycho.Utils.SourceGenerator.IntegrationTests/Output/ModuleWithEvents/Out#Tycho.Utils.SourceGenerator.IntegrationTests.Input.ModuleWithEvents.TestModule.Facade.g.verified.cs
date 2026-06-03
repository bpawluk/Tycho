//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents.TestModule.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents
{
    public class TestModuleFacade : ModuleFacadeBase, ITestModule
    {
        public TestModuleFacade(IModule<TestModule> module) : base(module) { }
    }
}
