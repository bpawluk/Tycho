using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class GetTransientServiceUsageRequestHandler(ITransientService service)
    : IHandle<GetAppTransientServiceUsageRequest, int>
    , IHandle<GetModuleTransientServiceUsageRequest, int>
{
    private readonly ITransientService _service = service;

    public Task<int> Handle(GetAppTransientServiceUsageRequest requestData, CancellationToken cancellationToken) =>
        Task.FromResult(_service.NumberOfCalls);

    public Task<int> Handle(GetModuleTransientServiceUsageRequest requestData, CancellationToken cancellationToken) =>
        Task.FromResult(_service.NumberOfCalls);
}
