//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName.Beta.TestApp.Builder.g.cs
#nullable enable annotations

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName
{
    public partial class Beta
    {
        public class TestAppBuilder
        {
            private readonly global::Tycho.Apps.IAppBuilderBase _appBuilderBase;

            public TestAppBuilder(global::Tycho.Apps.IAppBuilderBase appBuilderBase)
            {
                _appBuilderBase = appBuilderBase;
            }

            public ITestApp Build(global::System.IServiceProvider? parentServiceProvider = null)
            {
                global::Tycho.Apps.Instance.IApp app = _appBuilderBase.Build(parentServiceProvider);
                return new TestAppFacade(app);
            }
        }
    }
}
