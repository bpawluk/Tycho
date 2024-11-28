using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Handlers;

internal class AlphaRequestHandler(TestWorkflow<TestResult> testWorkflow)
    : IRequestHandler<AlphaRequest>
    , IRequestHandler<AlphaRequestWithResponse, string>
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task Handle(AlphaRequest requestData, CancellationToken cancellationToken)
    {
        _testWorkflow.SetResult(requestData.Result);
        return Task.CompletedTask;
    }

    public Task<string> Handle(AlphaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        _testWorkflow.SetResult(requestData.Result);
        return Task.FromResult("Test = Passed");
    }
}