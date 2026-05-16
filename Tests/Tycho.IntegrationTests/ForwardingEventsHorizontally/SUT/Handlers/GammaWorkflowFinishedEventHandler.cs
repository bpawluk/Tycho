using Tycho.Events;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Gamma;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Handlers;

internal class GammaWorkflowFinishedEventHandler(TestWorkflow<TestResult> testWorkflow, CompoundResult<string> result)
    : IEventHandler<GammaWorkflowFinishedEvent>
{
    private readonly CompoundResult<string> _compoundResult = result;
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task HandleAsync(EventContext<GammaWorkflowFinishedEvent> context, CancellationToken cancellationToken)
    {
        _compoundResult.AddSubResult(nameof(GammaModule));

        if (_compoundResult.IsComplete)
        {
            _testWorkflow.SetResult(context.Payload.Result);
        }

        return Task.CompletedTask;
    }
}
