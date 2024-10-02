using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Gamma;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Handlers;

internal class GammaOutRequestHandler(TestWorkflow<TestResult> testWorkflow) 
    : IHandle<GammaOutRequest>
    , IHandle<GammaOutRequestWithResponse, string>
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task Handle(GammaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        _testWorkflow.SetResult(requestData.Result);
        return Task.CompletedTask;
    }

    public Task<string> Handle(GammaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        _testWorkflow.SetResult(requestData.Result);
        return Task.FromResult("Test = Passed");
    }
}
