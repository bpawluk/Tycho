using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Gamma;
using Tycho.Requests;
using Tycho.TestUtils.Workflows;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Handlers;

internal class GammaOutRequestHandler(TestWorkflow<TestResult> testWorkflow)
    : IRequestHandler<GammaOutRequest>
    , IRequestHandler<GammaOutRequestWithResponse, string>
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task HandleAsync(GammaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        _testWorkflow.SetResult(requestData.Result);
        return Task.CompletedTask;
    }

    public Task<string> HandleAsync(GammaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        _testWorkflow.SetResult(requestData.Result);
        return Task.FromResult("Test = Passed");
    }
}
