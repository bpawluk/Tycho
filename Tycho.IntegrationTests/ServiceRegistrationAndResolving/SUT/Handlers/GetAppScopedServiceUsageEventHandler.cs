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

    public Task Handle(GetAppScopedServiceUsageEvent eventData, CancellationToken cancellationToken)
    {
        var firstServiceInstance = _serviceProvider.GetRequiredService<IScopedService>();
        _ = firstServiceInstance.NumberOfCalls;

        var secondServiceInstance = _serviceProvider.GetRequiredService<IScopedService>();
        eventData.Result.NumberOfCalls = secondServiceInstance.NumberOfCalls;

        _testWorkflow.SetResult(eventData.Result);
        return Task.CompletedTask;
    }
}