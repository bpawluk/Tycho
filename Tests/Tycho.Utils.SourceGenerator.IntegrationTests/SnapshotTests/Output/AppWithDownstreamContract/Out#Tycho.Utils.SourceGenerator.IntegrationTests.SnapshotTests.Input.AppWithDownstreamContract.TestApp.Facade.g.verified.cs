//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.TestApp.Facade.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract
{
    internal class TestAppFacade : global::Tycho.Apps.Instance.AppFacadeBase, ITestApp
    {
        public TestAppFacade(global::Tycho.Apps.Instance.IApp app) : base(app) { }

        public global::System.Threading.Tasks.Task<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.Requests.GetItemQuery.Result> ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.Requests.GetItemQuery requestData, global::System.Threading.CancellationToken cancellationToken)
        {
            return ExecuteAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.Requests.GetItemQuery, global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.Requests.GetItemQuery.Result>(requestData, cancellationToken);
        }    

        public global::System.Threading.Tasks.Task ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.Requests.DeleteItemCommand requestData, global::System.Threading.CancellationToken cancellationToken)
        {
            return ExecuteAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.Requests.DeleteItemCommand>(requestData, cancellationToken);
        }    
    }
}
