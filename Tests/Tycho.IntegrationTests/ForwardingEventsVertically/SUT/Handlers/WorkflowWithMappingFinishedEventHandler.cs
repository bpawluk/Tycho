using Tycho.Events;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Handlers;

internal class WorkflowWithMappingFinishedEventHandler(TestWorkflow<TestResult> testWorkflow) 
    : IEventHandler<WorkflowWithMappingFinishedEvent>
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task HandleAsync(EventContext<WorkflowWithMappingFinishedEvent> context, CancellationToken cancellationToken)
    {
        _testWorkflow.SetResult(context.Payload.Result);
        return Task.CompletedTask;
    }
}