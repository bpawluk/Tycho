using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class GetAppScopedServiceUsageRequestHandler(IServiceProvider serviceProvider)
    : IRequestHandler<GetAppScopedServiceUsageRequest, int>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task<int> HandleAsync(GetAppScopedServiceUsageRequest requestData, CancellationToken cancellationToken)
    {
        IScopedService firstServiceInstance = _serviceProvider.GetRequiredService<IScopedService>();
        _ = firstServiceInstance.NumberOfCalls;

        IScopedService secondServiceInstance = _serviceProvider.GetRequiredService<IScopedService>();
        int secondNumberOfCalls = secondServiceInstance.NumberOfCalls;

        return Task.FromResult(secondNumberOfCalls);
    }
}
