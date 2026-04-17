//HintName: Outer.Inner.TestModule.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

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
