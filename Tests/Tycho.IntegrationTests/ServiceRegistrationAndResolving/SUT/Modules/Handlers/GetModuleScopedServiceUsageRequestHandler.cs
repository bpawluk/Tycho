using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules.Handlers;

internal class GetModuleScopedServiceUsageRequestHandler(IServiceProvider serviceProvider)
    : IRequestHandler<GetModuleScopedServiceUsageRequest, int>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task<int> HandleAsync(GetModuleScopedServiceUsageRequest requestData, CancellationToken cancellationToken)
    {
        IScopedService firstServiceInstance = _serviceProvider.GetRequiredService<IScopedService>();
        _ = firstServiceInstance.NumberOfCalls;

        IScopedService secondServiceInstance = _serviceProvider.GetRequiredService<IScopedService>();
        int secondNumberOfCalls = secondServiceInstance.NumberOfCalls;

        return Task.FromResult(secondNumberOfCalls);
    }
}
