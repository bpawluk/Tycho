using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules.Handlers;

internal class GetModuleTransientServiceUsageRequestHandler(IServiceProvider serviceProvider)
    : IRequestHandler<GetModuleTransientServiceUsageRequest, int>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task<int> HandleAsync(GetModuleTransientServiceUsageRequest requestData, CancellationToken cancellationToken)
    {
        ITransientService firstServiceInstance = _serviceProvider.GetRequiredService<ITransientService>();
        _ = firstServiceInstance.NumberOfCalls;

        ITransientService secondServiceInstance = _serviceProvider.GetRequiredService<ITransientService>();
        int secondNumberOfCalls = secondServiceInstance.NumberOfCalls;

        return Task.FromResult(secondNumberOfCalls);
    }
}
