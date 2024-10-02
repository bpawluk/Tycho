using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class GetAppTransientServiceUsageRequestHandler(ITransientService service) : IHandle<GetAppTransientServiceUsageRequest, int>
{
    private readonly ITransientService _service = service;

    public Task<int> Handle(GetAppTransientServiceUsageRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_service.NumberOfCalls);
    }
}
