using IntegrationTests.ForwardingMessages.SUT;
using System.Threading.Tasks;
using Tycho;

namespace IntegrationTests.ForwardingMessages;

public record TestResult(string Id, int PreInterceptions, int PostInterceptions);

public class ForwardingMessagesTests : IAsyncLifetime
{
    private const string _requestResponse = "request-response";
    private const int _numberOfInterceptions = 4;
    private readonly TaskCompletionSource<TestResult> _testWorkflowTcs;
    private IModule? _sut;

    public ForwardingMessagesTests()
    {
        _testWorkflowTcs = new TaskCompletionSource<TestResult>();
    }

    public async Task InitializeAsync()
    {
        _sut = await new AppModule()
            .FulfillContract(consumer =>
            {
                consumer.Events.Handle<EventToForward>(eventData => _testWorkflowTcs.SetResult(new(eventData.Id, eventData.PreInterceptions, eventData.PostInterceptions)))
                        .Events.Handle<MappedEvent>(eventData => _testWorkflowTcs.SetResult(new(eventData.Id, eventData.PreInterceptions, eventData.PostInterceptions)))
                        .Requests.Handle<RequestToForward>(requestData => _testWorkflowTcs.SetResult(new(requestData.Id, requestData.PreInterceptions, requestData.PostInterceptions)))
                        .Requests.Handle<MappedRequest>(requestData => _testWorkflowTcs.SetResult(new(requestData.Id, requestData.PreInterceptions, requestData.PostInterceptions)))
                        .Requests.Handle<RequestWithResponseToForward, string>(requestData =>
                        {
                            _testWorkflowTcs.SetResult(new(requestData.Id, requestData.PreInterceptions, requestData.PostInterceptions));
                            return _requestResponse;
                        })
                        .Requests.Handle<MappedRequestWithResponse, string>(requestData =>
                        {
                            _testWorkflowTcs.SetResult(new(requestData.Id, requestData.PreInterceptions, requestData.PostInterceptions));
                            return _requestResponse;
                        });
            })
            .Build();
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingEvents()
    {
        // Arrange
        var workflowId = "event-workflow";
        var message = new EventToForward(workflowId, 0, 0);

        // Act
        _sut!.Publish(message);
        var result = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_numberOfInterceptions, result.PreInterceptions);
        Assert.Equal(_numberOfInterceptions, message.PostInterceptions);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingEventsWithMapping()
    {
        // Arrange
        var workflowId = "event-mapping-workflow";
        var message = new EventToForwardWithMapping(workflowId, 0, 0);

        // Act
        _sut!.Publish(message);
        var result = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_numberOfInterceptions, result.PreInterceptions);
        // PostInterceptions counter lost due to mapping
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingRequests()
    {
        // Arrange
        var workflowId = "request-workflow";
        var message = new RequestToForward(workflowId, 0, 0);

        // Act
        await _sut!.Execute(message);
        var result = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_numberOfInterceptions, result.PreInterceptions);
        Assert.Equal(_numberOfInterceptions, message.PostInterceptions);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingRequestsWithMapping()
    {
        // Arrange
        var workflowId = "request-mapping-workflow";
        var message = new RequestToForwardWithMapping(workflowId, 0, 0);

        // Act
        await _sut!.Execute(message);
        var result = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_numberOfInterceptions, result.PreInterceptions);
        // PostInterceptions counter lost due to mapping
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingQueries()
    {
        // Arrange
        var workflowId = "request-workflow";
        var message = new RequestWithResponseToForward(workflowId, 0, 0);

        // Act
        var response = await _sut!.Execute<RequestWithResponseToForward, string>(message);
        var result = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(_requestResponse, response);
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_numberOfInterceptions, result.PreInterceptions);
        Assert.Equal(_numberOfInterceptions, message.PostInterceptions);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingQueriesWithMapping()
    {
        // Arrange
        var workflowId = "request-mapping-workflow";
        var message = new RequestWithResponseToForwardWithMapping(workflowId, 0, 0);

        // Act
        var response = await _sut!.Execute<RequestWithResponseToForwardWithMapping, string>(message);
        var result = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(_requestResponse, response);
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_numberOfInterceptions, result.PreInterceptions);
        // PostInterceptions counter lost due to mapping
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
