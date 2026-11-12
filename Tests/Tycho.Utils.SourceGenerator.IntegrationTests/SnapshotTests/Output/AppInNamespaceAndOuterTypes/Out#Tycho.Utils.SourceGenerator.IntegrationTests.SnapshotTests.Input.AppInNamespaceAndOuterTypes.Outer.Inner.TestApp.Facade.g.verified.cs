//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespaceAndOuterTypes.Outer.Inner.TestApp.Facade.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespaceAndOuterTypes
{
    public partial class Outer
    {
        public partial class Inner
        {
            internal class TestAppFacade : global::Tycho.Apps.Instance.AppFacadeBase, ITestApp
            {
                public TestAppFacade(global::Tycho.Apps.Instance.IApp app) : base(app) { }
            }
        }
    }
}
