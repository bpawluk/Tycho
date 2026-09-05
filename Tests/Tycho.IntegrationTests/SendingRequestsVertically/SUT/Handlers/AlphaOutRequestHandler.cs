using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Alpha;
using Tycho.IntegrationTests._Utils;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Handlers;

internal class AlphaOutRequestHandler(TestWorkflow<TestResult> testWorkflow)
    : IRequestHandler<AlphaOutRequest>
    , IRequestHandler<AlphaOutRequestWithResponse, string>
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task HandleAsync(AlphaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        _testWorkflow.SetResult(requestData.Result);
        return Task.CompletedTask;
    }

    public Task<string> HandleAsync(AlphaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        _testWorkflow.SetResult(requestData.Result);
        return Task.FromResult("Test = Passed");
    }
}
