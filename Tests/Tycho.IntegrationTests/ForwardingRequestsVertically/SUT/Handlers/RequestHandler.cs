using Tycho.IntegrationTests._Utils;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Handlers;

internal class RequestHandler(TestWorkflow<TestResult> testWorkflow)
    : IRequestHandler<Request>
    , IRequestHandler<RequestWithResponse, string>
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    public Task HandleAsync(Request requestData, CancellationToken cancellationToken)
    {
        _testWorkflow.SetResult(requestData.Result);
        return Task.CompletedTask;
    }

    public Task<string> HandleAsync(RequestWithResponse requestData, CancellationToken cancellationToken)
    {
        _testWorkflow.SetResult(requestData.Result);
        return Task.FromResult("Test = Passed");
    }
}
