//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract.TestModule.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract
{
    internal class TestModuleFacade : ModuleFacadeBase, ITestModule
    {
        public TestModuleFacade(IModule<TestModule> module) : base(module) { }

        public Task<GetItemQuery.Result> ExecuteAsync(GetItemQuery requestData, CancellationToken cancellationToken)
        {
            return ExecuteAsync<GetItemQuery, GetItemQuery.Result>(requestData, cancellationToken);
        }

        public Task ExecuteAsync(DeleteItemCommand requestData, CancellationToken cancellationToken)
        {
            return ExecuteAsync<DeleteItemCommand>(requestData, cancellationToken);
        }
    }
}
