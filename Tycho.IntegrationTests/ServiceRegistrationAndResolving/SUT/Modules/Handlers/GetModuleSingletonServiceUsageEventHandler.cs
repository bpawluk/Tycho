using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Structure;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules.Handlers;

internal class GetModuleSingletonServiceUsageEventHandler(IParent parent, IServiceProvider serviceProvider)
    : IEventHandler<GetModuleSingletonServiceUsageEvent>
{
    private readonly IParent _parent = parent;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task Handle(GetModuleSingletonServiceUsageEvent eventData, CancellationToken cancellationToken)
    {
        var firstServiceInstance = _serviceProvider.GetRequiredService<ISingletonService>();
        _ = firstServiceInstance.NumberOfCalls;

        var secondServiceInstance = _serviceProvider.GetRequiredService<ISingletonService>();
        eventData.Result.NumberOfCalls = secondServiceInstance.NumberOfCalls;

        return _parent.Execute(new EndTestWorkflowRequest(eventData.Result), cancellationToken);
    }
}