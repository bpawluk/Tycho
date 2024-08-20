using System.Threading.Tasks;
using IntegrationTests.SendingMessagesDownTheHierarchy.SUT;
using Tycho;

namespace IntegrationTests.SendingMessagesDownTheHierarchy; 

public class SendingMessagesDownTheHierarchyTests : IAsyncLifetime
{
    private readonly TaskCompletionSource<string> _testWorkflowTcs;
    private IModule? _sut;

    public SendingMessagesDownTheHierarchyTests()
    {
        _testWorkflowTcs = new TaskCompletionSource<string>();
    }

    public async Task InitializeAsync()
    {
        _sut = await new AppModule()
            .FulfillContract(consumer =>
            {
                consumer.Events.Handle<EventWorkflowCompletedEvent>(eventData => _testWorkflowTcs.SetResult(eventData.Id));
                consumer.Events.Handle<CommandWorkflowCompletedEvent>(commandData => _testWorkflowTcs.SetResult(commandData.Id));
                consumer.Events.Handle<QueryWorkflowCompletedEvent>(queryData => _testWorkflowTcs.SetResult(queryData.Id));
            })
            .Build();
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_PassingEventsDownTheHierarchy()
    {
        // Arrange
        var workflowId = "event-workflow";

        // Act
        await _sut!.Execute<BeginEventWorkflowCommand>(new(workflowId));
        var returnedId = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, returnedId);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_PassingCommandsDownTheHierarchy()
    {
        // Arrange
        var workflowId = "command-workflow";

        // Act
        await _sut!.Execute<BeginCommandWorkflowCommand>(new(workflowId));
        var returnedId = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, returnedId);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_PassingQueriesDownTheHierarchy()
    {
        // Arrange
        var workflowId = "query-workflow";

        // Act
        await _sut!.Execute<BeginQueryWorkflowCommand>(new(workflowId));
        var returnedId = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, returnedId);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
