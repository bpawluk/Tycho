//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract.TestModule.Parent.Interface.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract
{
    public interface ITestModuleParent
    {
        global::System.Threading.Tasks.Task<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract.Requests.GetParentDataQuery.Result> ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract.Requests.GetParentDataQuery requestData, global::System.Threading.CancellationToken cancellationToken = default);

        global::System.Threading.Tasks.Task ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract.Requests.NotifyParentCommand requestData, global::System.Threading.CancellationToken cancellationToken = default);
    }
}
