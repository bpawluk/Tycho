//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithUpstreamContract.TestModule.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithUpstreamContract.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithUpstreamContract
{
    internal class TestModuleParent : ParentBase, TestModule.IParent
    {
        public TestModuleParent(IParentReference parentReference) : base(parentReference) { }

        public Task<GetParentDataQuery.Result> ExecuteAsync(GetParentDataQuery requestData, CancellationToken cancellationToken)
        {
            return ExecuteAsync<GetParentDataQuery, GetParentDataQuery.Result>(requestData, cancellationToken);
        }

        public Task ExecuteAsync(NotifyParentCommand requestData, CancellationToken cancellationToken)
        {
            return ExecuteAsync<NotifyParentCommand>(requestData, cancellationToken);
        }
    }
}
