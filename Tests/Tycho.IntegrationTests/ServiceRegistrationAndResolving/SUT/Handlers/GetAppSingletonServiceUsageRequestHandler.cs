using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class GetAppSingletonServiceUsageRequestHandler(IServiceProvider serviceProvider)
    : IRequestHandler<GetAppSingletonServiceUsageRequest, int>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task<int> HandleAsync(GetAppSingletonServiceUsageRequest requestData, CancellationToken cancellationToken)
    {
        ISingletonService firstServiceInstance = _serviceProvider.GetRequiredService<ISingletonService>();
        _ = firstServiceInstance.NumberOfCalls;

        ISingletonService secondServiceInstance = _serviceProvider.GetRequiredService<ISingletonService>();
        int secondNumberOfCalls = secondServiceInstance.NumberOfCalls;

        return Task.FromResult(secondNumberOfCalls);
    }
}
