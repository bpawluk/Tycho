using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Submodules.TestModule.Handlers;

internal class GetModuleTransientServiceUsageRequestHandler(ITransientService service) : IHandle<GetModuleTransientServiceUsageRequest, int>
{
    private readonly ITransientService _service = service;

    public Task<int> Handle(GetModuleTransientServiceUsageRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_service.NumberOfCalls);
    }
}
