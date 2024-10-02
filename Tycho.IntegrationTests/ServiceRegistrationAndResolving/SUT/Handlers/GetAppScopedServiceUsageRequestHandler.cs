using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class GetAppScopedServiceUsageRequestHandler(IScopedService service) : IHandle<GetAppScopedServiceUsageRequest, int>
{
    private readonly IScopedService _service = service;

    public Task<int> Handle(GetAppScopedServiceUsageRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_service.NumberOfCalls);
    }
}
