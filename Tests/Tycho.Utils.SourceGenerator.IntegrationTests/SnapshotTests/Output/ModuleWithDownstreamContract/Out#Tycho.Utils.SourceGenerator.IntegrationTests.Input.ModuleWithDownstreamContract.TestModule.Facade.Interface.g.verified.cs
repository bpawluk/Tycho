//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract.TestModule.Facade.Interface.g.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract
{
    public interface ITestModule : IRunnable, IDisposable
    {
        Task<GetItemQuery.Result> ExecuteAsync(GetItemQuery requestData, CancellationToken cancellationToken = default);

        Task ExecuteAsync(DeleteItemCommand requestData, CancellationToken cancellationToken = default);
    }
}
