using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules.Handlers;

internal class GetModuleSingletonServiceUsageEventHandler(ITestModuleParent parent, IServiceProvider serviceProvider)
    : IEventHandler<GetModuleSingletonServiceUsageEvent>
{
    private readonly ITestModuleParent _parent = parent;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task HandleAsync(EventContext<GetModuleSingletonServiceUsageEvent> context, CancellationToken cancellationToken)
    {
        ISingletonService firstServiceInstance = _serviceProvider.GetRequiredService<ISingletonService>();
        _ = firstServiceInstance.NumberOfCalls;

        ISingletonService secondServiceInstance = _serviceProvider.GetRequiredService<ISingletonService>();
        context.Payload.Result.NumberOfCalls = secondServiceInstance.NumberOfCalls;

        return _parent.ExecuteAsync(new EndTestWorkflowRequest(context.Payload.Result), cancellationToken);
    }
}
