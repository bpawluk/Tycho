using System.Threading;
using System.Threading.Tasks;
using Test.Integration.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Messaging.Handlers;

namespace Test.Integration.ServiceRegistrationAndResolving.SUT.Handlers;

internal class SingletonServiceWorkflowQueryHandler : IQueryHandler<SingletonServiceWorkflowQuery, int>
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
