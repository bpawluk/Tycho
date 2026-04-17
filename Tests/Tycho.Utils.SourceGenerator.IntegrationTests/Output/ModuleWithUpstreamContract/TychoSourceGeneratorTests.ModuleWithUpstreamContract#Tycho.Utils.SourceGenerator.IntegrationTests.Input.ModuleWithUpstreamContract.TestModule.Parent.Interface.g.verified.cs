//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithUpstreamContract.TestModule.Parent.Interface.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithUpstreamContract.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithUpstreamContract
{
    public partial class TestModule : TychoModule
    {
        public interface IParent
        {
            Task<GetParentDataQuery.Result> ExecuteAsync(GetParentDataQuery requestData, CancellationToken cancellationToken = default);

            Task ExecuteAsync(NotifyParentCommand requestData, CancellationToken cancellationToken = default);
        }
    }
}
