using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class GetAppTransientServiceUsageRequestHandler(IServiceProvider serviceProvider)
    : IRequestHandler<GetAppTransientServiceUsageRequest, int>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task<int> HandleAsync(GetAppTransientServiceUsageRequest requestData, CancellationToken cancellationToken)
    {
        ITransientService firstServiceInstance = _serviceProvider.GetRequiredService<ITransientService>();
        _ = firstServiceInstance.NumberOfCalls;

        ITransientService secondServiceInstance = _serviceProvider.GetRequiredService<ITransientService>();
        int secondNumberOfCalls = secondServiceInstance.NumberOfCalls;

        return Task.FromResult(secondNumberOfCalls);
    }
}
