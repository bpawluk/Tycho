using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class ModuleResolvingWorkflowQueryHandler : IRequestHandler<ModuleResolvingWorkflowQuery, string>
{
    private readonly IModule _module;

    public ModuleResolvingWorkflowQueryHandler(IModule module)
    {
        _module = module;
    }

    public Task<string> Handle(ModuleResolvingWorkflowQuery query, CancellationToken cancellationToken)
    {
        return _module.Execute<GetDataFromThisModulesClientQuery, string>(new(), cancellationToken);
    }
}
