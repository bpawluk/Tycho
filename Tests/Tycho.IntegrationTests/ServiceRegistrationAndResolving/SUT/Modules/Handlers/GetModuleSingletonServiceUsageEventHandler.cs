using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Structure.External;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules.Handlers;

internal class GetModuleSingletonServiceUsageEventHandler(IParentReference parent, IServiceProvider serviceProvider)
    : IEventHandler<GetModuleSingletonServiceUsageEvent>
{
    private readonly IParentReference _parent = parent;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task HandleAsync(EventContext<GetModuleSingletonServiceUsageEvent> context, CancellationToken cancellationToken)
    {
        var firstServiceInstance = _serviceProvider.GetRequiredService<ISingletonService>();
        _ = firstServiceInstance.NumberOfCalls;

        var secondServiceInstance = _serviceProvider.GetRequiredService<ISingletonService>();
        context.Payload.Result.NumberOfCalls = secondServiceInstance.NumberOfCalls;

        return Task.CompletedTask;
        //return _parent.ExecuteAsync(new EndTestWorkflowRequest(context.Payload.Result), cancellationToken);
    }
}