using IntegrationTests.ForwardingMessagesHorizontally.SUT;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ForwardingMessagesHorizontally;

internal record TestResult
{
    public string Id { get; init; } = default!;

    public int PreInterceptions { get; set; }

    public int PostInterceptions { get; set; }
}

internal class TestResultHandler :
    IEventHandler<EventToForward>,
    IEventHandler<MappedEvent>,
    IRequestHandler<RequestToForward>,
    IRequestHandler<MappedRequest>,
    IRequestHandler<RequestWithResponseToForward, string>,
    IRequestHandler<MappedRequestWithResponse, string>
{
    private readonly TaskCompletionSource<TestResult> _testWorkflowTcs = new();

    public const string ReturnedRespone = "returned-response";

    public async Task<TestResult> GetTestResult() => await _testWorkflowTcs.Task;

    public Task Handle(EventToForward eventData, CancellationToken cancellationToken)
    {
        _testWorkflowTcs.SetResult(eventData.Result);
        return Task.CompletedTask;
    }

    public Task Handle(MappedEvent eventData, CancellationToken cancellationToken)
    {
        _testWorkflowTcs.SetResult(eventData.Result);
        return Task.CompletedTask;
    }

    public Task Handle(RequestToForward requestData, CancellationToken cancellationToken)
    {
        _testWorkflowTcs.SetResult(requestData.Result);
        return Task.CompletedTask;
    }

    public Task Handle(MappedRequest requestData, CancellationToken cancellationToken)
    {
        _testWorkflowTcs.SetResult(requestData.Result);
        return Task.CompletedTask;
    }

    public Task<string> Handle(RequestWithResponseToForward requestData, CancellationToken cancellationToken)
    {
        _testWorkflowTcs.SetResult(requestData.Result);
        return Task.FromResult(ReturnedRespone);
    }

    public Task<string> Handle(MappedRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        _testWorkflowTcs.SetResult(requestData.Result);
        return Task.FromResult(ReturnedRespone);
    }
}
