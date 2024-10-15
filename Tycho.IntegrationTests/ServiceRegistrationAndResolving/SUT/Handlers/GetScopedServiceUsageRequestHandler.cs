using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class GetScopedServiceUsageRequestHandler(IScopedService service)
    : IRequestHandler<GetAppScopedServiceUsageRequest, int>
    , IRequestHandler<GetModuleScopedServiceUsageRequest, int>
{
    private readonly IScopedService _service = service;

    public Task<int> Handle(GetAppScopedServiceUsageRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_service.NumberOfCalls);
    }

    public Task<int> Handle(GetModuleScopedServiceUsageRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_service.NumberOfCalls);
    }
}