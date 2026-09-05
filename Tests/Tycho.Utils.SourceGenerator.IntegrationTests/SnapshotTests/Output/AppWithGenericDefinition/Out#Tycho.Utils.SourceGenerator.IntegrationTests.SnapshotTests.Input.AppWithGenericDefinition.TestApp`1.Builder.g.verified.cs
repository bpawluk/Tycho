//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithGenericDefinition.TestApp`1.Builder.g.cs
#nullable enable annotations

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithGenericDefinition
{
    public class TestAppBuilder<T>
    {
        private readonly global::Tycho.Apps.IAppBuilderBase _appBuilderBase;

        public TestAppBuilder(global::Tycho.Apps.IAppBuilderBase appBuilderBase)
        {
            _appBuilderBase = appBuilderBase;
        }

        public ITestApp<T> Build(global::System.IServiceProvider? parentServiceProvider = null)
        {
            global::Tycho.Apps.Instance.IApp app = _appBuilderBase.Build(parentServiceProvider);
            return new TestAppFacade<T>(app);
        }
    }
}
