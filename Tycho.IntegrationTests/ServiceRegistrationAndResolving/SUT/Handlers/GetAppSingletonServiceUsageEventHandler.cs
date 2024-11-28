using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class GetAppSingletonServiceUsageEventHandler(IServiceProvider serviceProvider, TestWorkflow<TestResult> testWorkflow)
    : IEventHandler<GetAppSingletonServiceUsageEvent>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task Handle(GetAppSingletonServiceUsageEvent eventData, CancellationToken cancellationToken)
    {
        var firstServiceInstance = _serviceProvider.GetRequiredService<ISingletonService>();
        _ = firstServiceInstance.NumberOfCalls;

        var secondServiceInstance = _serviceProvider.GetRequiredService<ISingletonService>();
        eventData.Result.NumberOfCalls = secondServiceInstance.NumberOfCalls;

        _testWorkflow.SetResult(eventData.Result);
        return Task.CompletedTask;
    }
}