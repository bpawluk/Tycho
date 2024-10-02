using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class GetAppSingletonServiceUsageRequestHandler(ISingletonService service) : IHandle<GetAppSingletonServiceUsageRequest, int>
{
    private readonly ISingletonService _service = service;

    public Task<int> Handle(GetAppSingletonServiceUsageRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_service.NumberOfCalls);
    }
}
