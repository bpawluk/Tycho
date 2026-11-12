using Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules;
using Tycho.Requests;
using Tycho.TestUtils.Workflows;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Handlers;

internal class AlphaRequestHandler(TestWorkflow<TestResult> testWorkflow)
    : IRequestHandler<AlphaRequest>
    , IRequestHandler<AlphaRequestWithResponse, string>
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task HandleAsync(AlphaRequest requestData, CancellationToken cancellationToken)
    {
        _testWorkflow.SetResult(requestData.Result);
        return Task.CompletedTask;
    }

    public Task<string> HandleAsync(AlphaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        _testWorkflow.SetResult(requestData.Result);
        return Task.FromResult("Test = Passed");
    }
}
