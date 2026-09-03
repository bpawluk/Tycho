//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.TestModule.Facade.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract
{
    internal class TestModuleFacade : global::Tycho.Modules.Instance.ModuleFacadeBase, ITestModule
    {
        public TestModuleFacade(global::Tycho.Modules.Instance.IModule<TestModule> module) : base(module) { }

        public global::System.Threading.Tasks.Task<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.Requests.GetItemQuery.Result> ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.Requests.GetItemQuery requestData, global::System.Threading.CancellationToken cancellationToken)
        {
            return ExecuteAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.Requests.GetItemQuery, global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.Requests.GetItemQuery.Result>(requestData, cancellationToken);
        }

        public global::System.Threading.Tasks.Task ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.Requests.DeleteItemCommand requestData, global::System.Threading.CancellationToken cancellationToken)
        {
            return ExecuteAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.Requests.DeleteItemCommand>(requestData, cancellationToken);
        }
    }
}
