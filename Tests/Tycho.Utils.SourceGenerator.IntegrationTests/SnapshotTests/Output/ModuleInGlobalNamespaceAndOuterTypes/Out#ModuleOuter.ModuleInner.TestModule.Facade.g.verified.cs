//HintName: ModuleOuter.ModuleInner.TestModule.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

public partial class ModuleOuter
{
    public partial class ModuleInner
    {
        internal class TestModuleFacade : ModuleFacadeBase, ITestModule
        {
            public TestModuleFacade(IModule<TestModule> module) : base(module) { }
        }
    }
}
