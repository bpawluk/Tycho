//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithGenericDefinition.TestModule`1.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithGenericDefinition
{
    public class TestModuleFacade<T> : ModuleFacadeBase, ITestModule<T>
    {
        public TestModuleFacade(IModule<TestModule<T>> module) : base(module) { }
    }
}
