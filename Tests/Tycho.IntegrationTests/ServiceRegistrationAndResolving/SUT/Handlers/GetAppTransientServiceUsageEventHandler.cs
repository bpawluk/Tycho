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

    public Task HandleAsync(EventContext<GetAppTransientServiceUsageEvent> context, CancellationToken cancellationToken)
    {
        var firstServiceInstance = _serviceProvider.GetRequiredService<ITransientService>();
        _ = firstServiceInstance.NumberOfCalls;

        var secondServiceInstance = _serviceProvider.GetRequiredService<ITransientService>();
        context.Payload.Result.NumberOfCalls = secondServiceInstance.NumberOfCalls;

        _testWorkflow.SetResult(context.Payload.Result);
        return Task.CompletedTask;
    }
}