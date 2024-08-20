using System.Threading;
using System.Threading.Tasks;
using IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class SingletonServiceWorkflowQueryHandler : IRequestHandler<SingletonServiceWorkflowQuery, int>
{
    private readonly ISingletonService _service;

    public SingletonServiceWorkflowQueryHandler(ISingletonService service)
    {
        _service = service;
    }

    public Task<int> Handle(SingletonServiceWorkflowQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(_service.NumberOfCalls);
    }
}
