using System.Threading.Tasks;
using Test.Integration.PassingMessagesBetweenSubmodules.SUT;
using Test.Integration.PassingMessagesBetweenSubmodules.SUT.Submodules;
using Tycho;

namespace Test.Integration.PassingMessagesBetweenSubmodules;

public class PassingMessagesBetweenSubmodulesTests : IAsyncLifetime
{
    private readonly TaskCompletionSource<string> _eventWorkflowTcs; 
    private readonly TaskCompletionSource<string> _commandWorkflowTcs; 
    private readonly TaskCompletionSource<string> _queryWorkflowTcs; 
    private IModule? _sut;

    public PassingMessagesBetweenSubmodulesTests()
    {
        _eventWorkflowTcs = new TaskCompletionSource<string>();
        _commandWorkflowTcs = new TaskCompletionSource<string>();
        _queryWorkflowTcs = new TaskCompletionSource<string>();
    }

    public async Task InitializeAsync()
    {
        _sut = await new AppModule()
            .FulfillContract(consumer =>
            {
                consumer.HandleEvent<EventWorkflowCompletedEvent>(eventData => _eventWorkflowTcs.SetResult(eventData.Id));
                consumer.HandleEvent<CommandWorkflowCompletedEvent>(commandData => _commandWorkflowTcs.SetResult(commandData.Id));
                consumer.HandleEvent<QueryWorkflowCompletedEvent>(queryData => _queryWorkflowTcs.SetResult(queryData.Id));
            })
            .Build();
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_PassingEventsBetweenSubmodules()
    {
        // Arrange
        var workflowId = "event-workflow";

        // Act
        await _sut!.Execute<BeginEventWorkflowCommand>(new(workflowId));
        var returnedId = await _eventWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, returnedId);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_PassingCommandsBetweenSubmodules()
    {
        // Arrange
        var workflowId = "command-workflow";

        // Act
        await _sut!.Execute<BeginCommandWorkflowCommand>(new(workflowId));
        var returnedId = await _commandWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, returnedId);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_PassingQueriesBetweenSubmodules()
    {
        // Arrange
        var workflowId = "query-workflow";

        // Act
        await _sut!.Execute<BeginQueryWorkflowCommand>(new(workflowId));
        var returnedId = await _queryWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, returnedId);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
