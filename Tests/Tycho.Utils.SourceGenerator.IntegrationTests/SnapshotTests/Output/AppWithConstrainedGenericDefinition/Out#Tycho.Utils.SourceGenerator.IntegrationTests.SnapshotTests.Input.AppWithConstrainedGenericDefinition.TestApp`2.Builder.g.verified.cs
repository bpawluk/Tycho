//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.TestApp`2.Builder.g.cs
#nullable enable annotations

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition
{
    public class TestAppBuilder<TPayload, TKey>
        where TPayload : global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.Model.PayloadBase, global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.IMarker, new()
        where TKey : notnull
    {
        private readonly global::Tycho.Apps.IAppBuilderBase _appBuilderBase;

        public TestAppBuilder(global::Tycho.Apps.IAppBuilderBase appBuilderBase)
        {
            _appBuilderBase = appBuilderBase;
        }

        public ITestApp<TPayload, TKey> Build(global::System.IServiceProvider? parentServiceProvider = null)
        {
            global::Tycho.Apps.Instance.IApp app = _appBuilderBase.Build(parentServiceProvider);
            return new TestAppFacade<TPayload, TKey>(app);
        }
    }
}
