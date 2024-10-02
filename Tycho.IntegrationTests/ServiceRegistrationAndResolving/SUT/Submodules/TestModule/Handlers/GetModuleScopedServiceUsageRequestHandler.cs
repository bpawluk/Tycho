using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Submodules.TestModule.Handlers;

internal class GetModuleScopedServiceUsageRequestHandler(IScopedService service) : IHandle<GetModuleScopedServiceUsageRequest, int>
{
    private readonly IScopedService _service = service;

    public Task<int> Handle(GetModuleScopedServiceUsageRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_service.NumberOfCalls);
    }
}
