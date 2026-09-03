//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithDownstreamContract.TestApp.Facade.Interface.g.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithDownstreamContract.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithDownstreamContract
{
    public interface ITestApp : IRunnable, IDisposable
    {
        Task<GetItemQuery.Result> ExecuteAsync(GetItemQuery requestData, CancellationToken cancellationToken = default);

        Task ExecuteAsync(DeleteItemCommand requestData, CancellationToken cancellationToken = default);
    }
}
