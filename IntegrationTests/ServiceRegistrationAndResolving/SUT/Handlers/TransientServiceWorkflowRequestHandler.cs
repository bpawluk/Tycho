using System.Threading;
using System.Threading.Tasks;
using IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class TransientServiceWorkflowRequestHandler(ITransientService service) : 
    IRequestHandler<TransientServiceWorkflowRequest, int>
{
    private readonly ITransientService _service = service;

    public Task<int> Handle(TransientServiceWorkflowRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_service.NumberOfCalls);
    }
}
