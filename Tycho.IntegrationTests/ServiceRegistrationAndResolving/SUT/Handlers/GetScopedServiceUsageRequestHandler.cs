using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class GetScopedServiceUsageRequestHandler(IScopedService service) 
    : IHandle<GetAppScopedServiceUsageRequest, int>
    , IHandle<GetModuleScopedServiceUsageRequest, int>
{
    private readonly IScopedService _service = service;

    public Task<int> Handle(GetAppScopedServiceUsageRequest requestData, CancellationToken cancellationToken) =>
        Task.FromResult(_service.NumberOfCalls);

    public Task<int> Handle(GetModuleScopedServiceUsageRequest requestData, CancellationToken cancellationToken) =>
        Task.FromResult(_service.NumberOfCalls);
}
