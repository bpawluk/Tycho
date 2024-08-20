using System.Threading;
using System.Threading.Tasks;
using IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class TransientServiceWorkflowQueryHandler : IRequestHandler<TransientServiceWorkflowQuery, int>
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
