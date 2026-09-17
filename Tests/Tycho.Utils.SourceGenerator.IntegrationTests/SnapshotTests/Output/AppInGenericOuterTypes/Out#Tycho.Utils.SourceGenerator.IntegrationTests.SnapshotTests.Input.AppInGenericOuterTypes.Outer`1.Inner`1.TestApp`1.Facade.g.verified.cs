//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInGenericOuterTypes.Outer`1.Inner`1.TestApp`1.Facade.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            internal class TestAppFacade<TApp> : global::Tycho.Apps.Instance.AppFacadeBase, ITestApp<TApp>
                where TApp : new()
            {
                public TestAppFacade(global::Tycho.Apps.Instance.IApp app) : base(app) { }
            }
        }
    }
}
