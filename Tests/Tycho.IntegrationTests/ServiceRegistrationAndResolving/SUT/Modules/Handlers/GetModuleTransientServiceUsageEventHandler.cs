using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules.Handlers;

internal class GetModuleTransientServiceUsageEventHandler(ITestModuleParent parent, IServiceProvider serviceProvider)
    : IEventHandler<GetModuleTransientServiceUsageEvent>
{
    private readonly ITestModuleParent _parent = parent;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task HandleAsync(EventContext<GetModuleTransientServiceUsageEvent> context, CancellationToken cancellationToken)
    {
        ITransientService firstServiceInstance = _serviceProvider.GetRequiredService<ITransientService>();
        _ = firstServiceInstance.NumberOfCalls;

        ITransientService secondServiceInstance = _serviceProvider.GetRequiredService<ITransientService>();
        context.Payload.Result.NumberOfCalls = secondServiceInstance.NumberOfCalls;

        return _parent.ExecuteAsync(new EndTestWorkflowRequest(context.Payload.Result), cancellationToken);
    }
}
