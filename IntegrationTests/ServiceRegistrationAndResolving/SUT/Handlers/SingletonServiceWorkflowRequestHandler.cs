using System.Threading;
using System.Threading.Tasks;
using IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class SingletonServiceWorkflowRequestHandler(ISingletonService service) : 
    IRequestHandler<SingletonServiceWorkflowRequest, int>
{
    private readonly ISingletonService _service = service;

    public Task<int> Handle(SingletonServiceWorkflowRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_service.NumberOfCalls);
    }
}
