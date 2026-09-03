//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithUpstreamContract.TestModule.Parent.Interface.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithUpstreamContract.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithUpstreamContract
{
    public interface ITestModuleParent
    {
        Task<GetParentDataQuery.Result> ExecuteAsync(GetParentDataQuery requestData, CancellationToken cancellationToken = default);

        Task ExecuteAsync(NotifyParentCommand requestData, CancellationToken cancellationToken = default);
    }
}
