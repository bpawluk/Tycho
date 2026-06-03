//HintName: TestModule.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

public class TestModuleFacade : ModuleFacadeBase, ITestModule
{
    public TestModuleFacade(IModule<TestModule> module) : base(module) { }
}
