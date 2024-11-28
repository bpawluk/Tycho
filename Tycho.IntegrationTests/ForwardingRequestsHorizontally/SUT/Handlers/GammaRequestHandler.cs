using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Gamma;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Handlers;

internal class GammaRequestHandler(TestWorkflow<TestResult> testWorkflow) 
    : IRequestHandler<GammaRequest>
    , IRequestHandler<GammaRequestWithResponse, string>
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task Handle(GammaRequest requestData, CancellationToken cancellationToken)
    {
        _testWorkflow.SetResult(requestData.Result);
        return Task.CompletedTask;
    }

    public Task<string> Handle(GammaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        _testWorkflow.SetResult(requestData.Result);
        return Task.FromResult("Test = Passed");
    }
}