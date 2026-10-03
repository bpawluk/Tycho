//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInGenericOuterTypes.Outer`1.Inner`1.TestModule`1.Parent.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            internal class TestModuleParent<TModule> : global::Tycho.Structure.Parent.ParentBase, ITestModuleParent<TModule>
                where TModule : notnull
            {
                public TestModuleParent(global::Tycho.Structure.Parent.IParentReference parentReference) : base(parentReference) { }
            }
        }
    }
}
