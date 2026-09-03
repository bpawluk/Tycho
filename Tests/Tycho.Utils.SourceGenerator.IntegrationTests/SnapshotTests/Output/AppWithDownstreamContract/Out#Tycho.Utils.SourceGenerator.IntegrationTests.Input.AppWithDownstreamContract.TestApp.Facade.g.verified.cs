//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithDownstreamContract.TestApp.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Apps.Instance;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithDownstreamContract.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithDownstreamContract
{
    internal class TestAppFacade : AppFacadeBase, ITestApp
    {
        public TestAppFacade(IApp app) : base(app) { }

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
