using System.Threading;
using System.Threading.Tasks;
using IntegrationTests.ServiceRegistrationAndResolving.SUT.Submodules;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class SubmoduleResolvingWorkflowRequestHandler(IModule<AppSubmodule> submodule) : 
    IRequestHandler<SubmoduleResolvingWorkflowRequest, string>
{
    private readonly IModule _submodule = submodule;

    public Task<string> Handle(SubmoduleResolvingWorkflowRequest request, CancellationToken cancellationToken)
    {
        return _submodule.Execute<GetDataFromSubmoduleRequest, string>(new());
    }
}
