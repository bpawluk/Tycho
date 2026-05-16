using Tycho.Events;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Beta;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Handlers;

internal class BetaWorkflowFinishedEventHandler(TestWorkflow<TestResult> testWorkflow, CompoundResult<string> result)
    : IEventHandler<BetaWorkflowFinishedEvent>
{
    private readonly CompoundResult<string> _compoundResult = result;
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task HandleAsync(EventContext<BetaWorkflowFinishedEvent> context, CancellationToken cancellationToken)
    {
        _compoundResult.AddSubResult(nameof(BetaModule));

        if (_compoundResult.IsComplete)
        {
            _testWorkflow.SetResult(context.Payload.Result);
        }

        return Task.CompletedTask;
    }
}
