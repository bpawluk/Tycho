//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.TestModule.Facade.Interface.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract
{
    public interface ITestModule : global::Tycho.Structure.IRunnable, global::System.IDisposable
    {
        global::System.Threading.Tasks.Task<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.Requests.GetItemQuery.Result> ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.Requests.GetItemQuery requestData, global::System.Threading.CancellationToken cancellationToken = default);

        global::System.Threading.Tasks.Task ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.Requests.DeleteItemCommand requestData, global::System.Threading.CancellationToken cancellationToken = default);
    }
}
