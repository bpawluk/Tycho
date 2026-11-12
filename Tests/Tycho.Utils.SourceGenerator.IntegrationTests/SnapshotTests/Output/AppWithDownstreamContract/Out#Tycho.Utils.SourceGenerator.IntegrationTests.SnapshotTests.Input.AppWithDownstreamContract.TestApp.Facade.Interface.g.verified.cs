//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.TestApp.Facade.Interface.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract
{
    public interface ITestApp : global::Tycho.Structure.IRunnable, global::System.IDisposable
    {
        global::System.Threading.Tasks.Task<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.Requests.GetItemQuery.Result> ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.Requests.GetItemQuery requestData, global::System.Threading.CancellationToken cancellationToken = default);

        global::System.Threading.Tasks.Task ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.Requests.DeleteItemCommand requestData, global::System.Threading.CancellationToken cancellationToken = default);
    }
}
