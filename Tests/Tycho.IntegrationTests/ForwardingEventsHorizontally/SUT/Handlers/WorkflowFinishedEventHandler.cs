using Tycho.Events;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Handlers;

internal class WorkflowFinishedEventHandler(TestWorkflow<TestResult> testWorkflow, CompoundResult<string> result)
    : IEventHandler<WorkflowFinishedEvent>
{
    private readonly CompoundResult<string> _compoundResult = result;
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task HandleAsync(EventContext<WorkflowFinishedEvent> context, CancellationToken cancellationToken)
    {
        _compoundResult.AddSubResult(context.Payload.FinalModule);

        if (_compoundResult.IsComplete)
        {
            _testWorkflow.SetResult(context.Payload.Result);
        }

        return Task.CompletedTask;
    }
}
