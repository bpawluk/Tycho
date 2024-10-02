using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Submodules.TestModule.Handlers;

internal class GetModuleSingletonServiceUsageRequestHandler(ISingletonService service) : IHandle<GetModuleSingletonServiceUsageRequest, int>
{
    private readonly ISingletonService _service = service;

    public Task<int> Handle(GetModuleSingletonServiceUsageRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_service.NumberOfCalls);
    }
}
