using Tycho.IntegrationTests._Utils;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Handlers;

internal class RequestHandler(TestWorkflow<TestResult> testWorkflow) 
    : IRequestHandler<Request>
    , IRequestHandler<RequestWithResponse, string>
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task Handle(Request requestData, CancellationToken cancellationToken)
    {
        _testWorkflow.SetResult(requestData.Result);
        return Task.CompletedTask;
    }

    public Task<string> Handle(RequestWithResponse requestData, CancellationToken cancellationToken)
    {
        _testWorkflow.SetResult(requestData.Result);
        return Task.FromResult("Test = Passed");
    }
}
