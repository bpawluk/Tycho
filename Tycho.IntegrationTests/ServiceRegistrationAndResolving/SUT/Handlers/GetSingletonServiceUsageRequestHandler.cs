using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class GetSingletonServiceUsageRequestHandler(ISingletonService service)
    : IRequestHandler<GetAppSingletonServiceUsageRequest, int>
    , IRequestHandler<GetModuleSingletonServiceUsageRequest, int>
{
    private readonly ISingletonService _service = service;

    public Task<int> Handle(GetAppSingletonServiceUsageRequest requestData, CancellationToken cancellationToken) =>
        Task.FromResult(_service.NumberOfCalls);

    public Task<int> Handle(GetModuleSingletonServiceUsageRequest requestData, CancellationToken cancellationToken) =>
        Task.FromResult(_service.NumberOfCalls);
}
