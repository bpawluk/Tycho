using Tycho.Events;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Handlers;

internal class WorkflowFinishedEventHandler(TestWorkflow<TestResult> testWorkflow)
    : IEventHandler<WorkflowFinishedEvent>
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task HandleAsync(EventContext<WorkflowFinishedEvent> context, CancellationToken cancellationToken)
    {
        _testWorkflow.SetResult(context.Payload.Result);
        return Task.CompletedTask;
    }
}
