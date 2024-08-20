using System.Threading;
using System.Threading.Tasks;
using IntegrationTests.ServiceRegistrationAndResolving.SUT.Submodules;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class SubmoduleResolvingWorkflowQueryHandler : IRequestHandler<SubmoduleResolvingWorkflowQuery, string>
{
    private readonly IModule _submodule;

    public SubmoduleResolvingWorkflowQueryHandler(IModule<AppSubmodule> submodule)
    {
        _submodule = submodule;
    }

    public Task<string> Handle(SubmoduleResolvingWorkflowQuery query, CancellationToken cancellationToken = default)
    {
        return _submodule.Execute<GetDataFromSubmoduleQuery, string>(new());
    }
}
