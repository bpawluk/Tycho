using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class ModuleResolvingWorkflowRequestHandler(IModule module) : 
    IRequestHandler<ModuleResolvingWorkflowRequest, string>
{
    private readonly IModule _module = module;

    public Task<string> Handle(ModuleResolvingWorkflowRequest request, CancellationToken cancellationToken)
    {
        return _module.Execute<GetDataFromThisModulesClientRequestWithResponse, string>(new(), cancellationToken);
    }
}
