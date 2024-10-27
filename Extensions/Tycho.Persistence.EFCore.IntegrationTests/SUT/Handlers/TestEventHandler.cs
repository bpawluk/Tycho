using Tycho.Events;
using Tycho.Persistence.EFCore.IntegrationTests._Utils;

namespace Tycho.Persistence.EFCore.IntegrationTests.SUT.Handlers;

internal class TestEventHandler(TestWorkflow<TestResult> testWorkflow) : IEventHandler<TestEvent>
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task Handle(TestEvent eventData, CancellationToken cancellationToken)
    {
        _testWorkflow.SetResult(eventData.Result);
        return Task.CompletedTask;
    }
}