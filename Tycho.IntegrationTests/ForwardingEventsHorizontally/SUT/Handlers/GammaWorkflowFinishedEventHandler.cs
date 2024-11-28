using Tycho.Events;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Gamma;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Handlers;

internal class GammaWorkflowFinishedEventHandler(TestWorkflow<TestResult> testWorkflow, CompoundResult<Type> result)
    : IEventHandler<GammaWorkflowFinishedEvent>
{
    private readonly CompoundResult<Type> _compoundResult = result;
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task Handle(GammaWorkflowFinishedEvent eventData, CancellationToken cancellationToken = default)
    {
        _compoundResult.AddSubResult(typeof(GammaModule));

        if (_compoundResult.IsComplete)
        {
            _testWorkflow.SetResult(eventData.Result);
        }

        return Task.CompletedTask;
    }
}