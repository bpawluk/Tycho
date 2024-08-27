using System.Threading;
using System.Threading.Tasks;
using IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class ScopedServiceWorkflowRequestHandler(IScopedService service) : 
    IRequestHandler<ScopedServiceWorkflowRequest, int>
{
    private readonly IScopedService _service = service;

    public Task<int> Handle(ScopedServiceWorkflowRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_service.NumberOfCalls);
    }
}
