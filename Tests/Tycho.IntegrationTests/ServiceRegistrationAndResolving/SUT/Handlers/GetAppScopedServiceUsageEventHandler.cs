using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class GetAppScopedServiceUsageEventHandler(IServiceProvider serviceProvider, TestWorkflow<TestResult> testWorkflow)
    : IEventHandler<GetAppScopedServiceUsageEvent>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task HandleAsync(EventContext<GetAppScopedServiceUsageEvent> context, CancellationToken cancellationToken)
    {
        IScopedService firstServiceInstance = _serviceProvider.GetRequiredService<IScopedService>();
        _ = firstServiceInstance.NumberOfCalls;

        IScopedService secondServiceInstance = _serviceProvider.GetRequiredService<IScopedService>();
        context.Payload.Result.NumberOfCalls = secondServiceInstance.NumberOfCalls;

        _testWorkflow.SetResult(context.Payload.Result);
        return Task.CompletedTask;
    }
}
