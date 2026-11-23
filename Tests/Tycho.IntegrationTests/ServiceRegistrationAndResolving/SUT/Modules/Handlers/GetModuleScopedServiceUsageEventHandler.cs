using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules.Handlers;

internal class GetModuleScopedServiceUsageEventHandler(ITestModuleParent parent, IServiceProvider serviceProvider)
    : IEventHandler<GetModuleScopedServiceUsageEvent>
{
    private readonly ITestModuleParent _parent = parent;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task HandleAsync(EventContext<GetModuleScopedServiceUsageEvent> context, CancellationToken cancellationToken)
    {
        IScopedService firstServiceInstance = _serviceProvider.GetRequiredService<IScopedService>();
        _ = firstServiceInstance.NumberOfCalls;

        IScopedService secondServiceInstance = _serviceProvider.GetRequiredService<IScopedService>();
        context.Payload.Result.NumberOfCalls = secondServiceInstance.NumberOfCalls;

        return _parent.ExecuteAsync(new EndTestWorkflowRequest(context.Payload.Result), cancellationToken);
    }
}
