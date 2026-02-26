using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Structure.External;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules.Handlers;

internal class GetModuleTransientServiceUsageEventHandler(IParent parent, IServiceProvider serviceProvider)
    : IEventHandler<GetModuleTransientServiceUsageEvent>
{
    private readonly IParent _parent = parent;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task HandleAsync(EventContext<GetModuleTransientServiceUsageEvent> context, CancellationToken cancellationToken)
    {
        var firstServiceInstance = _serviceProvider.GetRequiredService<ITransientService>();
        _ = firstServiceInstance.NumberOfCalls;

        var secondServiceInstance = _serviceProvider.GetRequiredService<ITransientService>();
        context.Payload.Result.NumberOfCalls = secondServiceInstance.NumberOfCalls;

        return Task.CompletedTask;
        //return _parent.ExecuteAsync(new EndTestWorkflowRequest(context.Payload.Result), cancellationToken);
    }
}