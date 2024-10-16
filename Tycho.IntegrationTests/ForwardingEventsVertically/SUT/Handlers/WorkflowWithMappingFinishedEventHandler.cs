using Tycho.Events;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Handlers;

internal class WorkflowWithMappingFinishedEventHandler(TestWorkflow<TestResult> testWorkflow) 
    : IEventHandler<WorkflowWithMappingFinishedEvent>
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task Handle(WorkflowWithMappingFinishedEvent eventData, CancellationToken cancellationToken)
    {
        _testWorkflow.SetResult(eventData.Result);
        return Task.CompletedTask;
    }
}