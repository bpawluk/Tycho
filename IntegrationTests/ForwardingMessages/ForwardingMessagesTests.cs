using IntegrationTests.ForwardingMessages.SUT;
using System.Threading.Tasks;
using Tycho;

namespace IntegrationTests.ForwardingMessages;

public record TestResult(string Id, int PreInterceptions, int PostInterceptions);

public class ForwardingMessagesTests : IAsyncLifetime
{
    private const string _queryResponse = "query-response";
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
                        .Requests.Handle<CommandToForward>(commandData => _testWorkflowTcs.SetResult(new(commandData.Id, commandData.PreInterceptions, commandData.PostInterceptions)))
                        .Requests.Handle<MappedCommand>(commandData => _testWorkflowTcs.SetResult(new(commandData.Id, commandData.PreInterceptions, commandData.PostInterceptions)))
                        .Requests.Handle<QueryToForward, string>(queryData =>
                        {
                            _testWorkflowTcs.SetResult(new(queryData.Id, queryData.PreInterceptions, queryData.PostInterceptions));
                            return _queryResponse;
                        })
                        .Requests.Handle<MappedQuery, string>(queryData =>
                        {
                            _testWorkflowTcs.SetResult(new(queryData.Id, queryData.PreInterceptions, queryData.PostInterceptions));
                            return _queryResponse;
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
    public async Task Tycho_Enables_ForwardingCommands()
    {
        // Arrange
        var workflowId = "command-workflow";
        var message = new CommandToForward(workflowId, 0, 0);

        // Act
        await _sut!.Execute(message);
        var result = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_numberOfInterceptions, result.PreInterceptions);
        Assert.Equal(_numberOfInterceptions, message.PostInterceptions);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingCommandsWithMapping()
    {
        // Arrange
        var workflowId = "command-mapping-workflow";
        var message = new CommandToForwardWithMapping(workflowId, 0, 0);

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
        var workflowId = "query-workflow";
        var message = new QueryToForward(workflowId, 0, 0);

        // Act
        var response = await _sut!.Execute<QueryToForward, string>(message);
        var result = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(_queryResponse, response);
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_numberOfInterceptions, result.PreInterceptions);
        Assert.Equal(_numberOfInterceptions, message.PostInterceptions);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingQueriesWithMapping()
    {
        // Arrange
        var workflowId = "query-mapping-workflow";
        var message = new QueryToForwardWithMapping(workflowId, 0, 0);

        // Act
        var response = await _sut!.Execute<QueryToForwardWithMapping, string>(message);
        var result = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(_queryResponse, response);
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_numberOfInterceptions, result.PreInterceptions);
        // PostInterceptions counter lost due to mapping
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
