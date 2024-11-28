using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class GetAppTransientServiceUsageEventHandler(IServiceProvider serviceProvider, TestWorkflow<TestResult> testWorkflow)
    : IEventHandler<GetAppTransientServiceUsageEvent>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task Handle(GetAppTransientServiceUsageEvent eventData, CancellationToken cancellationToken)
    {
        var firstServiceInstance = _serviceProvider.GetRequiredService<ITransientService>();
        _ = firstServiceInstance.NumberOfCalls;

        var secondServiceInstance = _serviceProvider.GetRequiredService<ITransientService>();
        eventData.Result.NumberOfCalls = secondServiceInstance.NumberOfCalls;

        _testWorkflow.SetResult(eventData.Result);
        return Task.CompletedTask;
    }
}