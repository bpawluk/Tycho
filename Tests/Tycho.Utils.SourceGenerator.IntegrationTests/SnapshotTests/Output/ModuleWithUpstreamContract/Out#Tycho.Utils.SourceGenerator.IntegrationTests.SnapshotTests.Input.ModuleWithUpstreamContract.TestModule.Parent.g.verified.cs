//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract.TestModule.Parent.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract
{
    internal class TestModuleParent : global::Tycho.Structure.Parent.ParentBase, ITestModuleParent
    {
        public TestModuleParent(global::Tycho.Structure.Parent.IParentReference parentReference) : base(parentReference) { }

        public global::System.Threading.Tasks.Task<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract.Requests.GetParentDataQuery.Result> ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract.Requests.GetParentDataQuery requestData, global::System.Threading.CancellationToken cancellationToken)
        {
            return ExecuteAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract.Requests.GetParentDataQuery, global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract.Requests.GetParentDataQuery.Result>(requestData, cancellationToken);
        }

        public global::System.Threading.Tasks.Task ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract.Requests.NotifyParentCommand requestData, global::System.Threading.CancellationToken cancellationToken)
        {
            return ExecuteAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract.Requests.NotifyParentCommand>(requestData, cancellationToken);
        }
    }
}
