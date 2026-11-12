//HintName: ModuleOuter.ModuleInner.TestModule.Facade.g.cs
public partial class ModuleOuter
{
    public partial class ModuleInner
    {
        internal class TestModuleFacade : global::Tycho.Modules.Instance.ModuleFacadeBase, ITestModule
        {
            public TestModuleFacade(global::Tycho.Modules.Instance.IModule<TestModule> module) : base(module) { }
        }
    }
}
