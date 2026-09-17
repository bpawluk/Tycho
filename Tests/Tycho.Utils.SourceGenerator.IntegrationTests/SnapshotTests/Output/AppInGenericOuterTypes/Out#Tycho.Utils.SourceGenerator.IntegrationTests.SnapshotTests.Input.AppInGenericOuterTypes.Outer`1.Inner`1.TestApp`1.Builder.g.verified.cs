//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInGenericOuterTypes.Outer`1.Inner`1.TestApp`1.Builder.g.cs
#nullable enable annotations

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            public class TestAppBuilder<TApp>
                where TApp : new()
            {
                private readonly global::Tycho.Apps.IAppBuilderBase _appBuilderBase;

                public TestAppBuilder(global::Tycho.Apps.IAppBuilderBase appBuilderBase)
                {
                    _appBuilderBase = appBuilderBase;
                }

                public ITestApp<TApp> Build(global::System.IServiceProvider? parentServiceProvider = null)
                {
                    global::Tycho.Apps.Instance.IApp app = _appBuilderBase.Build(parentServiceProvider);
                    return new TestAppFacade<TApp>(app);
                }
            }
        }
    }
}
