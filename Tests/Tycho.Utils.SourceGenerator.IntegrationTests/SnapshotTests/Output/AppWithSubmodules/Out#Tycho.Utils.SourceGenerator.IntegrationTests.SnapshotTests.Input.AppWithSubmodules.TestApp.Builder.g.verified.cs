//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules.TestApp.Builder.g.cs
#nullable enable annotations

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules
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
