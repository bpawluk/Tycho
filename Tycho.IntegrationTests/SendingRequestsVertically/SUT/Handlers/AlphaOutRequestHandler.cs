using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Alpha;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Handlers;

internal class AlphaOutRequestHandler(TestWorkflow<TestResult> testWorkflow) 
    : IRequestHandler<AlphaOutRequest>
    , IRequestHandler<AlphaOutRequestWithResponse, string>
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task Handle(AlphaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        _testWorkflow.SetResult(requestData.Result);
        return Task.CompletedTask;
    }

    public Task<string> Handle(AlphaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        _testWorkflow.SetResult(requestData.Result);
        return Task.FromResult("Test = Passed");
    }
}
