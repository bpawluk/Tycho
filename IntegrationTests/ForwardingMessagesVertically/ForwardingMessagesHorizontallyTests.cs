using IntegrationTests.ForwardingMessagesVertically.SUT;
using System.Threading.Tasks;
using Tycho;

namespace IntegrationTests.ForwardingMessagesVertically;

public class ForwardingMessagesVerticallyTests : IAsyncLifetime
{
    private const int _expectedNumberOfInterceptions = 4;

    private readonly TestResultHandler _testResultHandler = new();
    private IModule? _sut;

    public async Task InitializeAsync()
    {
        _sut = await new AppModule()
            .FulfillContract(appOutbox =>
            {
                appOutbox.Events.Handle<EventToForward>(_testResultHandler)
                         .Events.Handle<MappedEvent>(_testResultHandler);

                appOutbox.Requests.Handle<RequestToForward>(_testResultHandler)
                         .Requests.Handle<MappedRequest>(_testResultHandler);

                appOutbox.Requests.Handle<RequestWithResponseToForward, string>(_testResultHandler)
                         .Requests.Handle<MappedRequestWithResponse, string>(_testResultHandler);
            })
            .Build();
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingEventsVertically()
    {
        // Arrange
        var workflowId = "event-workflow";
        var message = new EventToForward(new() { Id = workflowId });

        // Act
        _sut!.Publish(message);
        var result = await _testResultHandler.GetTestResult();

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_expectedNumberOfInterceptions, result.PreInterceptions);
        Assert.Equal(_expectedNumberOfInterceptions, result.PostInterceptions);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingEventsVerticallyWithMapping()
    {
        // Arrange
        var workflowId = "event-mapping-workflow";
        var message = new EventToForwardWithMapping(new() { Id = workflowId });

        // Act
        _sut!.Publish(message);
        var result = await _testResultHandler.GetTestResult();

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_expectedNumberOfInterceptions, result.PreInterceptions);
        Assert.Equal(_expectedNumberOfInterceptions, result.PostInterceptions);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingRequestsVertically()
    {
        // Arrange
        var workflowId = "request-workflow";
        var message = new RequestToForward(new() { Id = workflowId });

        // Act
        await _sut!.Execute(message);
        var result = await _testResultHandler.GetTestResult();

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_expectedNumberOfInterceptions, result.PreInterceptions);
        Assert.Equal(_expectedNumberOfInterceptions, result.PostInterceptions);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingRequestsVerticallyWithMapping()
    {
        // Arrange
        var workflowId = "request-mapping-workflow";
        var message = new RequestToForwardWithMapping(new() { Id = workflowId });

        // Act
        await _sut!.Execute(message);
        var result = await _testResultHandler.GetTestResult();

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_expectedNumberOfInterceptions, result.PreInterceptions);
        Assert.Equal(_expectedNumberOfInterceptions, result.PostInterceptions);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingRequestsWithResponsesVertically()
    {
        // Arrange
        var workflowId = "request-workflow";
        var message = new RequestWithResponseToForward(new() { Id = workflowId });

        // Act
        var response = await _sut!.Execute<RequestWithResponseToForward, string>(message);
        var result = await _testResultHandler.GetTestResult();

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_expectedNumberOfInterceptions, result.PreInterceptions);
        Assert.Equal(_expectedNumberOfInterceptions, result.PostInterceptions);
        Assert.Equal(TestResultHandler.ReturnedRespone, response);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_ForwardingRequestsWithResponsesVerticallyWithMapping()
    {
        // Arrange
        var workflowId = "request-mapping-workflow";
        var message = new RequestWithResponseToForwardWithMapping(new() { Id = workflowId });

        // Act
        var response = await _sut!.Execute<RequestWithResponseToForwardWithMapping, string>(message);
        var result = await _testResultHandler.GetTestResult();

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_expectedNumberOfInterceptions, result.PreInterceptions);
        Assert.Equal(_expectedNumberOfInterceptions, result.PostInterceptions);
        Assert.Equal(TestResultHandler.ReturnedRespone, response);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
