//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInNamespaceAndOuterTypes.Outer.Inner.TestModule.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInNamespaceAndOuterTypes
{
    public partial class Outer
    {
        public partial class Inner
        {
            internal class TestModuleFacade : ModuleFacadeBase, ITestModule
            {
                public TestModuleFacade(IModule<TestModule> module) : base(module) { }
            }
        }
    }
}
