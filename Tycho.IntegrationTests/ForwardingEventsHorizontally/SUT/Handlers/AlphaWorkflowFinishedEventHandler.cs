using Tycho.Events;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Handlers;

internal class AlphaWorkflowFinishedEventHandler(TestWorkflow<TestResult> testWorkflow, CompoundResult<Type> result)
    : IEventHandler<AlphaWorkflowFinishedEvent>
{
    private readonly CompoundResult<Type> _compoundResult = result;
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task Handle(AlphaWorkflowFinishedEvent eventData, CancellationToken cancellationToken = default)
    {
        _compoundResult.AddSubResult(typeof(AlphaModule));

        if (_compoundResult.IsComplete)
        {
            _testWorkflow.SetResult(eventData.Result);
        }

        return Task.CompletedTask;
    }
}