using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Structure;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules.Handlers;

internal class GetModuleScopedServiceUsageEventHandler(IParent parent, IServiceProvider serviceProvider)
    : IEventHandler<GetModuleScopedServiceUsageEvent>
{
    private readonly IParent _parent = parent;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task HandleAsync(EventContext<GetModuleScopedServiceUsageEvent> context, CancellationToken cancellationToken)
    {
        var firstServiceInstance = _serviceProvider.GetRequiredService<IScopedService>();
        _ = firstServiceInstance.NumberOfCalls;

        var secondServiceInstance = _serviceProvider.GetRequiredService<IScopedService>();
        context.Payload.Result.NumberOfCalls = secondServiceInstance.NumberOfCalls;

        return _parent.ExecuteAsync(new EndTestWorkflowRequest(context.Payload.Result), cancellationToken);
    }
}