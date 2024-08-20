using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using IntegrationTests.ContractDefinitionAndMessageHandling.SUT;
using IntegrationTests.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;
using Tycho;

namespace IntegrationTests.ContractDefinitionAndMessageHandling;

public class ContractDefinitionAndMessageHandlingTests : IAsyncLifetime
{
    private IModule? _sut;

    public bool EventWorkflowCompleted { get; set; } = false;
    public bool RequestWorkflowCompleted { get; set; } = false;
    public string RequestWithResponseResponse => "sample-response";

    public async Task InitializeAsync()
    {
        var externalServiceCollection = new ServiceCollection().AddSingleton(this);
        _sut = await new AppModule()
            .FulfillContract(consumer =>
            {
                consumer.Events.Handle<HandledByLambdaEvent>(_ => EventWorkflowCompleted = true)
                        .Events.Handle<HandledByAsyncLambdaEvent>((_, _) =>
                        {
                            EventWorkflowCompleted = true;
                            return Task.CompletedTask;
                        })
                        .Events.Handle(new HandledByHandlerInstanceEventHandler(this))
                        .Events.Handle<HandledByHandlerTypeEvent, HandledByHandlerTypeEventHandler>();

                consumer.Requests.Handle<HandledByLambdaRequest>(_ => RequestWorkflowCompleted = true)
                        .Requests.Handle<HandledByAsyncLambdaRequest>((_, _) =>
                        {
                            RequestWorkflowCompleted = true;
                            return Task.CompletedTask;
                        })
                        .Requests.Handle(new HandledByHandlerInstanceRequestHandler(this))
                        .Requests.Handle<HandledByHandlerTypeRequest, HandledByHandlerTypeRequestHandler>();

                consumer.Requests.Handle<HandledByLambdaRequestWithResponse, string>(_ => RequestWithResponseResponse)
                        .Requests.Handle<HandledByAsyncLambdaRequestWithResponse, string>((_, _) =>
                        {
                            return Task.FromResult(RequestWithResponseResponse);
                        })
                        .Requests.Handle(new HandledByHandlerInstanceRequestWithResponseHandler(this))
                        .Requests.Handle<HandledByHandlerTypeRequestWithResponse, string, HandledByHandlerTypeRequestWithResponseHandler>();
            }, externalServiceCollection.BuildServiceProvider())
            .Build();
    }

    [Fact]
    public async Task Tycho_EnablesHandlingMessages_WithLambdas()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut!.Publish<HandledByLambdaEvent>(new());
        await _sut.Execute<HandledByLambdaRequest>(new());
        var requestResponse = await _sut.Execute<HandledByLambdaRequestWithResponse, string>(new());

        // Assert
        Assert.True(EventWorkflowCompleted);
        Assert.True(RequestWorkflowCompleted);
        Assert.Equal(RequestWithResponseResponse, requestResponse);

    }

    [Fact]
    public async Task Tycho_EnablesHandlingMessages_WithAsyncLambdas()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut!.Publish<HandledByAsyncLambdaEvent>(new());
        await _sut.Execute<HandledByAsyncLambdaRequest>(new());
        var requestResponse = await _sut.Execute<HandledByAsyncLambdaRequestWithResponse, string>(new());

        // Assert
        Assert.True(EventWorkflowCompleted);
        Assert.True(RequestWorkflowCompleted);
        Assert.Equal(RequestWithResponseResponse, requestResponse);
    }

    [Fact]
    public async Task Tycho_EnablesHandlingMessages_WithHandlerInstances()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut!.Publish<HandledByHandlerInstanceEvent>(new());
        await _sut.Execute<HandledByHandlerInstanceRequest>(new());
        var requestResponse = await _sut.Execute<HandledByHandlerInstanceRequestWithResponse, string>(new());

        // Assert
        Assert.True(EventWorkflowCompleted);
        Assert.True(RequestWorkflowCompleted);
        Assert.Equal(RequestWithResponseResponse, requestResponse);
    }

    [Fact]
    public async Task Tycho_EnablesHandlingMessages_WithHandlerTypes()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut!.Publish<HandledByHandlerTypeEvent>(new());
        await _sut.Execute<HandledByHandlerTypeRequest>(new());
        var requestResponse = await _sut.Execute<HandledByHandlerTypeRequestWithResponse, string>(new());

        // Assert
        Assert.True(EventWorkflowCompleted);
        Assert.True(RequestWorkflowCompleted);
        Assert.Equal(RequestWithResponseResponse, requestResponse);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
