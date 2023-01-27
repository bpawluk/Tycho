using System.Threading.Tasks;
using Test.Integration.ForwardingMessages.SUT;
using Test.Integration.ForwardingMessages.SUT.Submodules;
using Tycho;

namespace Test.Integration.ForwardingMessages;

public class ForwardingMessagesTests : IAsyncLifetime
{
    private const string _queryResponse = "query-response";
    private readonly TaskCompletionSource<string> _testWorkflowTcs;
    private IModule? _sut;

    public ForwardingMessagesTests()
    {
        _testWorkflowTcs = new TaskCompletionSource<string>();
    }

    public async Task InitializeAsync()
    {
        _sut = await new AppModule()
            .FulfillContract(consumer =>
            {
                consumer.HandleEvent<BetaEvent>(eventData => _testWorkflowTcs.SetResult(eventData.Id))
                        .HandleEvent<MappedEvent>(eventData => _testWorkflowTcs.SetResult(eventData.Id))
                        .HandleCommand<BetaCommand>(commandData => _testWorkflowTcs.SetResult(commandData.Id))
                        .HandleCommand<MappedCommand>(commandData => _testWorkflowTcs.SetResult(commandData.Id))
                        .HandleQuery<BetaQuery, string>(queryData =>
                        {
                            _testWorkflowTcs.SetResult(queryData.Id);
                            return _queryResponse;
                        })
                        .HandleQuery<MappedQuery, string>(queryData =>
                        {
                            _testWorkflowTcs.SetResult(queryData.Id);
                            return _queryResponse;
                        })
                        .HandleQuery<MappedQueryAndResponse, string>(queryData =>
                        {
                            _testWorkflowTcs.SetResult(queryData.Id);
                            return _queryResponse;
                        });
            })
            .Build();
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_PassingOnEvents()
    {
        // Arrange
        var workflowId = "event-workflow";

        // Act
        _sut!.Publish<EventToPass>(new(workflowId));
        var returnedId = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, returnedId);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_PassingOnEventsWithMapping()
    {
        // Arrange
        var workflowId = "event-mapping-workflow";

        // Act
        _sut!.Publish<EventToPassWithMapping>(new(workflowId));
        var returnedId = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, returnedId);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingCommands()
    {
        // Arrange
        var workflowId = "command-workflow";

        // Act
        await _sut!.Execute<CommandToForward>(new(workflowId));
        var returnedId = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, returnedId);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingCommandsWithMapping()
    {
        // Arrange
        var workflowId = "command-mapping-workflow";

        // Act
        await _sut!.Execute<CommandToForwardWithMapping>(new(workflowId));
        var returnedId = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, returnedId);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingQueries()
    {
        // Arrange
        var workflowId = "query-workflow";

        // Act
        var response = await _sut!.Execute<QueryToForward, string>(new(workflowId));
        var returnedId = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(_queryResponse, response);
        Assert.Equal(workflowId, returnedId);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingQueriesWithMessageMapping()
    {
        // Arrange
        var workflowId = "query-message-mapping-workflow";

        // Act
        var response = await _sut!.Execute<QueryToForwardWithMessageMapping, string>(new(workflowId));
        var returnedId = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(_queryResponse, response);
        Assert.Equal(workflowId, returnedId);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingQueriesWithMessageAndResponseMapping()
    {
        // Arrange
        var workflowId = "query-message-and-response-mapping-workflow";

        // Act
        var response = await _sut!.Execute<QueryToForwardWithMessageAndResponseMapping, string>(new(workflowId));
        var returnedId = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(_queryResponse, response);
        Assert.Equal(workflowId, returnedId);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
