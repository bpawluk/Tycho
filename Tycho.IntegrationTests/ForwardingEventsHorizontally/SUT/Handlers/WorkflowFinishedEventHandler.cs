using Tycho.IntegrationTests._Utils;
using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Handlers;

internal class WorkflowFinishedEventHandler(
    TestWorkflow<TestResult> testWorkflow, 
    CompoundResult<Type> result) 
    : IEventHandler<WorkflowFinishedEvent>
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;
    private readonly CompoundResult<Type> _compoundResult = result;

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
