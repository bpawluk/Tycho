//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestApp.Facade.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions
{
    internal class TestAppFacade : global::Tycho.Apps.Instance.AppFacadeBase, ITestApp
    {
        public TestAppFacade(global::Tycho.Apps.Instance.IApp app) : base(app) { }

        public global::System.Threading.Tasks.Task<global::System.String> ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestRequestFromHelperExtension requestData, global::System.Threading.CancellationToken cancellationToken)
        {
            return ExecuteAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestRequestFromHelperExtension, global::System.String>(requestData, cancellationToken);
        }    

        public global::System.Threading.Tasks.Task<global::System.String> ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestRequestFromHelperStaticClass requestData, global::System.Threading.CancellationToken cancellationToken)
        {
            return ExecuteAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestRequestFromHelperStaticClass, global::System.String>(requestData, cancellationToken);
        }    

        public global::System.Threading.Tasks.Task<global::System.String> ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestRequestFromHelperClass requestData, global::System.Threading.CancellationToken cancellationToken)
        {
            return ExecuteAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestRequestFromHelperClass, global::System.String>(requestData, cancellationToken);
        }    

        public global::System.Threading.Tasks.Task<global::System.String> ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestRequestFromLocalStaticHelper requestData, global::System.Threading.CancellationToken cancellationToken)
        {
            return ExecuteAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestRequestFromLocalStaticHelper, global::System.String>(requestData, cancellationToken);
        }    

        public global::System.Threading.Tasks.Task<global::System.String> ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestRequestFromLocalHelper requestData, global::System.Threading.CancellationToken cancellationToken)
        {
            return ExecuteAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestRequestFromLocalHelper, global::System.String>(requestData, cancellationToken);
        }    
    }
}
