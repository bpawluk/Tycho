using Tycho.Events;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Beta;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Handlers;

internal class BetaWorkflowFinishedEventHandler(TestWorkflow<TestResult> testWorkflow, CompoundResult<Type> result)
    : IEventHandler<BetaWorkflowFinishedEvent>
{
    private readonly CompoundResult<Type> _compoundResult = result;
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task Handle(BetaWorkflowFinishedEvent eventData, CancellationToken cancellationToken = default)
    {
        _compoundResult.AddSubResult(typeof(BetaModule));

        if (_compoundResult.IsComplete)
        {
            _testWorkflow.SetResult(eventData.Result);
        }

        return Task.CompletedTask;
    }
}