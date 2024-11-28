using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class GetAppSingletonServiceUsageRequestHandler(IServiceProvider serviceProvider)
    : IRequestHandler<GetAppSingletonServiceUsageRequest, int>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task<int> Handle(GetAppSingletonServiceUsageRequest requestData, CancellationToken cancellationToken)
    {
        var firstServiceInstance = _serviceProvider.GetRequiredService<ISingletonService>();
        _ = firstServiceInstance.NumberOfCalls;

        var secondServiceInstance = _serviceProvider.GetRequiredService<ISingletonService>();
        var secondNumberOfCalls = secondServiceInstance.NumberOfCalls;

        return Task.FromResult(secondNumberOfCalls);
    }
}