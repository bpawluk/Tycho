using System.Threading;
using System.Threading.Tasks;
using Test.Integration.ServiceRegistrationAndResolving.SUT.Submodules;
using Tycho;
using Tycho.Messaging.Handlers;

namespace Test.Integration.ServiceRegistrationAndResolving.SUT.Handlers;

internal class SubmoduleResolvingWorkflowQueryHandler : IQueryHandler<SubmoduleResolvingWorkflowQuery, string>
{
    private readonly IModule _submodule;

    public SubmoduleResolvingWorkflowQueryHandler(ISubmodule<AppSubmodule> submodule)
    {
        _submodule = submodule;
    }

    public Task<string> Handle(SubmoduleResolvingWorkflowQuery query, CancellationToken cancellationToken = default)
    {
        return _submodule.Execute<GetDataFromSubmoduleQuery, string>(new());
    }
}
