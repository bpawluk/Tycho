using System.Threading;
using System.Threading.Tasks;
using IntegrationTests.ServiceRegistrationAndResolving.SUT.Submodules;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class SubmoduleResolvingWorkflowRequestHandler : IRequestHandler<SubmoduleResolvingWorkflowRequest, string>
{
    private readonly IModule _submodule;

    public SubmoduleResolvingWorkflowRequestHandler(IModule<AppSubmodule> submodule)
    {
        _submodule = submodule;
    }

    public Task<string> Handle(SubmoduleResolvingWorkflowRequest request, CancellationToken cancellationToken = default)
    {
        return _submodule.Execute<GetDataFromSubmoduleRequestWithResponse, string>(new());
    }
}
