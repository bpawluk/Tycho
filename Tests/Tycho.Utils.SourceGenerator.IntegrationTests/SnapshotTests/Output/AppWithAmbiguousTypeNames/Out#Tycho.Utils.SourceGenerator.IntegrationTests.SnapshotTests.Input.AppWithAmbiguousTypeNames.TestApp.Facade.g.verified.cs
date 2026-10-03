//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames.TestApp.Facade.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames
{
    internal class TestAppFacade : global::Tycho.Apps.Instance.AppFacadeBase, ITestApp
    {
        public TestAppFacade(global::Tycho.Apps.Instance.IApp app) : base(app) { }

        public global::System.Threading.Tasks.Task ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames.Alpha.Request requestData, global::System.Threading.CancellationToken cancellationToken)
        {
            return ExecuteAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames.Alpha.Request>(requestData, cancellationToken);
        }    

        public global::System.Threading.Tasks.Task ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames.Beta.Request requestData, global::System.Threading.CancellationToken cancellationToken)
        {
            return ExecuteAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames.Beta.Request>(requestData, cancellationToken);
        }    
    }
}
