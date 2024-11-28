using Tycho.Events;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Handlers;

internal class WorkflowFinishedEventHandler(TestWorkflow<TestResult> testWorkflow, CompoundResult<Type> result)
    : IEventHandler<WorkflowFinishedEvent>
{
    private readonly CompoundResult<Type> _compoundResult = result;
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task Handle(WorkflowFinishedEvent eventData, CancellationToken cancellationToken = default)
    {
        _compoundResult.AddSubResult(eventData.FinalModule);

        if (_compoundResult.IsComplete)
        {
            _testWorkflow.SetResult(eventData.Result);
        }

        return Task.CompletedTask;
    }
}