//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithGenericDefinition.TestModule`1.Facade.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithGenericDefinition
{
    internal class TestModuleFacade<T> : global::Tycho.Modules.Instance.ModuleFacadeBase, ITestModule<T>
    {
        public TestModuleFacade(global::Tycho.Modules.Instance.IModule<TestModule<T>> module) : base(module) { }
    }
}
