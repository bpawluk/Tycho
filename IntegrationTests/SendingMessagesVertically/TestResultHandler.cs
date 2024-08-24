using IntegrationTests.SendingMessagesVertically.SUT;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.SendingMessagesVertically;

internal record TestResult
{
    public string Id { get; init; } = default!;

    public int HandlingCount { get; set; }
}

internal class TestResultHandler :
    IEventHandler<EventToSend>,
    IEventHandler<MappedEvent>,
    IRequestHandler<RequestToSend>,
    IRequestHandler<MappedRequest>,
    IRequestHandler<RequestWithResponseToSend, string>,
    IRequestHandler<MappedRequestWithResponse, string>
{
    private readonly TaskCompletionSource<TestResult> _testWorkflowTcs = new();

    public const string ReturnedRespone = "returned-response";

    public async Task<TestResult> GetTestResult() => await _testWorkflowTcs.Task;

    public Task Handle(EventToSend eventData, CancellationToken cancellationToken)
    {
        _testWorkflowTcs.SetResult(eventData.Result);
        return Task.CompletedTask;
    }

    public Task Handle(MappedEvent eventData, CancellationToken cancellationToken)
    {
        _testWorkflowTcs.SetResult(eventData.Result);
        return Task.CompletedTask;
    }

    public Task Handle(RequestToSend requestData, CancellationToken cancellationToken)
    {
        _testWorkflowTcs.SetResult(requestData.Result);
        return Task.CompletedTask;
    }

    public Task Handle(MappedRequest requestData, CancellationToken cancellationToken)
    {
        _testWorkflowTcs.SetResult(requestData.Result);
        return Task.CompletedTask;
    }

    public Task<string> Handle(RequestWithResponseToSend requestData, CancellationToken cancellationToken)
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
