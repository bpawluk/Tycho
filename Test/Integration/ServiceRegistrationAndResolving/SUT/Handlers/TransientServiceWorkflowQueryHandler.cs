using System.Threading;
using System.Threading.Tasks;
using Test.Integration.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Messaging.Handlers;

namespace Test.Integration.ServiceRegistrationAndResolving.SUT.Handlers;

internal class TransientServiceWorkflowQueryHandler : IQueryHandler<TransientServiceWorkflowQuery, int>
{
    private readonly ITransientService _service;

    public TransientServiceWorkflowQueryHandler(ITransientService service)
    {
        _service = service;
    }

    public Task<int> Handle(TransientServiceWorkflowQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(_service.NumberOfCalls);
    }
}
