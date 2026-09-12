//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInGenericOuterTypes.Outer`1.Inner`1.TestModule`1.Facade.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            internal class TestModuleFacade<TModule> : global::Tycho.Modules.Instance.ModuleFacadeBase, ITestModule<TModule>
                where TModule : notnull
            {
                public TestModuleFacade(global::Tycho.Modules.Instance.IModule<TestModule<TModule>> module) : base(module) { }
            }
        }
    }
}
