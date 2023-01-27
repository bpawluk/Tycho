﻿using System.Threading.Tasks;
using Test.Integration.PassingMessagesBetweenSubmodules.SUT;
using Test.Integration.PassingMessagesBetweenSubmodules.SUT.Submodules;
using Tycho;

namespace Test.Integration.PassingMessagesBetweenSubmodules;

public class PassingMessagesBetweenSubmodulesTests : IAsyncLifetime
{
    private readonly TaskCompletionSource<string> _testWorkflowTcs; 
    private IModule? _sut;

    public PassingMessagesBetweenSubmodulesTests()
    {
        _testWorkflowTcs = new TaskCompletionSource<string>();
    }

    public async Task InitializeAsync()
    {
        _sut = await new AppModule()
            .FulfillContract(consumer =>
            {
                consumer.HandleEvent<EventWorkflowCompletedEvent>(eventData => _testWorkflowTcs.SetResult(eventData.Id));
                consumer.HandleEvent<CommandWorkflowCompletedEvent>(commandData => _testWorkflowTcs.SetResult(commandData.Id));
                consumer.HandleEvent<QueryWorkflowCompletedEvent>(queryData => _testWorkflowTcs.SetResult(queryData.Id));
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
        var returnedId = await _testWorkflowTcs.Task;

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
        var returnedId = await _testWorkflowTcs.Task;

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
        var returnedId = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, returnedId);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
