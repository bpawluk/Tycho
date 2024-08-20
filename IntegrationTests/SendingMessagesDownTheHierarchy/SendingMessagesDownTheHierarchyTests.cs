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
                consumer.Events.Handle<RequestWorkflowCompletedEvent>(requestData => _testWorkflowTcs.SetResult(requestData.Id));
                consumer.Events.Handle<RequestWithResponseWorkflowCompletedEvent>(requestData => _testWorkflowTcs.SetResult(requestData.Id));
            })
            .Build();
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_PassingEventsDownTheHierarchy()
    {
        // Arrange
        var workflowId = "event-workflow";

        // Act
        await _sut!.Execute<BeginEventWorkflowRequest>(new(workflowId));
        var returnedId = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, returnedId);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_PassingRequestsDownTheHierarchy()
    {
        // Arrange
        var workflowId = "request-workflow";

        // Act
        await _sut!.Execute<BeginRequestWorkflowRequest>(new(workflowId));
        var returnedId = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, returnedId);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_PassingQueriesDownTheHierarchy()
    {
        // Arrange
        var workflowId = "request-workflow";

        // Act
        await _sut!.Execute<BeginRequestWithResponseWorkflowRequest>(new(workflowId));
        var returnedId = await _testWorkflowTcs.Task;

        // Assert
        Assert.Equal(workflowId, returnedId);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
