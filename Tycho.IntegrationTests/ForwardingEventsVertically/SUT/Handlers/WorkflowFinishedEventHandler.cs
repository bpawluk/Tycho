using Tycho.Events;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Handlers;

internal class WorkflowFinishedEventHandler(TestWorkflow<TestResult> testWorkflow) : IEventHandler<WorkflowFinishedEvent>
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task Handle(WorkflowFinishedEvent eventData, CancellationToken cancellationToken = default)
    {
        _testWorkflow.SetResult(eventData.Result);
        return Task.CompletedTask;
    }
}