using IntegrationTests.SendingMessagesVertically.SUT;
using System.Threading.Tasks;
using Tycho;

namespace IntegrationTests.SendingMessagesVertically;

public class SendingMessagesVerticallyTests : IAsyncLifetime
{
    private const int _expectedHandlingCount = 7;

    private readonly TestResultHandler _testResultHandler = new();
    private IModule? _sut;

    public async Task InitializeAsync()
    {
        _sut = await new AppModule()
            .FulfillContract(appOutbox =>
            {
                appOutbox.Events.Handle<EventToSend>(_testResultHandler)
                         .Events.Handle<MappedEvent>(_testResultHandler);

                appOutbox.Requests.Handle<RequestToSend>(_testResultHandler)
                         .Requests.Handle<MappedRequest>(_testResultHandler);

                appOutbox.Requests.Handle<RequestWithResponseToSend, string>(_testResultHandler)
                         .Requests.Handle<MappedRequestWithResponse, string>(_testResultHandler);
            })
            .Build();
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_SendingEventsVertically()
    {
        // Arrange
        var workflowId = "event-workflow";
        var message = new EventToSend(new() { Id = workflowId });

        // Act
        _sut!.Publish(message);
        var result = await _testResultHandler.GetTestResult();

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_expectedHandlingCount, result.HandlingCount);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_SendingEventsVerticallyWithMapping()
    {
        // Arrange
        var workflowId = "event-mapping-workflow";
        var message = new EventToSendWithMapping(new() { Id = workflowId });

        // Act
        _sut!.Publish(message);
        var result = await _testResultHandler.GetTestResult();

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_expectedHandlingCount, result.HandlingCount);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_SendingRequestsVertically()
    {
        // Arrange
        var workflowId = "request-workflow";
        var message = new RequestToSend(new() { Id = workflowId });

        // Act
        await _sut!.Execute(message);
        var result = await _testResultHandler.GetTestResult();

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_expectedHandlingCount, result.HandlingCount);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_SendingRequestsVerticallyWithMapping()
    {
        // Arrange
        var workflowId = "request-mapping-workflow";
        var message = new RequestToSendWithMapping(new() { Id = workflowId });

        // Act
        await _sut!.Execute(message);
        var result = await _testResultHandler.GetTestResult();

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_expectedHandlingCount, result.HandlingCount);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_SendingRequestsWithResponsesVertically()
    {
        // Arrange
        var workflowId = "request-workflow";
        var message = new RequestWithResponseToSend(new() { Id = workflowId });

        // Act
        var response = await _sut!.Execute<RequestWithResponseToSend, string>(message);
        var result = await _testResultHandler.GetTestResult();

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_expectedHandlingCount, result.HandlingCount);
        Assert.Equal(TestResultHandler.ReturnedRespone, response);
    }

    [Fact(Timeout = 1000)]
    public async Task Tycho_Enables_SendingRequestsWithResponsesVerticallyWithMapping()
    {
        // Arrange
        var workflowId = "request-mapping-workflow";
        var message = new RequestWithResponseToSendWithMapping(new() { Id = workflowId });

        // Act
        var response = await _sut!.Execute<RequestWithResponseToSendWithMapping, string>(message);
        var result = await _testResultHandler.GetTestResult();

        // Assert
        Assert.Equal(workflowId, result.Id);
        Assert.Equal(_expectedHandlingCount, result.HandlingCount);
        Assert.Equal(TestResultHandler.ReturnedRespone, response);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
