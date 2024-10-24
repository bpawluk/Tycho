using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;

internal class EndTestWorkflowRequestHandler(TestWorkflow<TestResult> testWorkflow) : IRequestHandler<EndTestWorkflowRequest>
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task Handle(EndTestWorkflowRequest requestData, CancellationToken cancellationToken = default)
    {
        _testWorkflow.SetResult(requestData.Result);
        return Task.CompletedTask;
    }
}