using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace Test.Integration.ServiceRegistrationAndResolving.SUT.Handlers;

internal class ModuleResolvingWorkflowQueryHandler : IQueryHandler<ModuleResolvingWorkflowQuery, string>
{
    private readonly IModule _module;

    public ModuleResolvingWorkflowQueryHandler(IModule module)
    {
        _module = module;
    }

    public Task<string> Handle(ModuleResolvingWorkflowQuery query, CancellationToken cancellationToken)
    {
        return _module.ExecuteQuery<GetDataFromThisModulesClientQuery, string>(new(), cancellationToken);
    }
}
