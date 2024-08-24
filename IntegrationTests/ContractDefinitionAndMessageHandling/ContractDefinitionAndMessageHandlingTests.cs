using IntegrationTests.ContractDefinitionAndMessageHandling.SUT;
using IntegrationTests.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Tycho;

namespace IntegrationTests.ContractDefinitionAndMessageHandling;

public class ContractDefinitionAndMessageHandlingTests : IAsyncLifetime
{
    private IModule? _sut;

    public const string ReturnedResponse = "sample-response";

    public bool EventWorkflowCompleted { get; set; } = false;
    public bool RequestWorkflowCompleted { get; set; } = false;

    public async Task InitializeAsync()
    {
        var externalServiceCollection = new ServiceCollection().AddSingleton(this);
        _sut = await new AppModule()
            .FulfillContract(consumer =>
            {
                consumer.Events.Handle(new HandledByHandlerInstanceEventHandler(this))
                        .Events.Handle<HandledByHandlerTypeEvent, HandledByHandlerTypeEventHandler>();

                consumer.Requests.Handle(new HandledByHandlerInstanceRequestHandler(this))
                        .Requests.Handle<HandledByHandlerTypeRequest, HandledByHandlerTypeRequestHandler>();

                consumer.Requests.Handle(new HandledByHandlerInstanceRequestWithResponseHandler())
                        .Requests.Handle<HandledByHandlerTypeRequestWithResponse, string, HandledByHandlerTypeRequestWithResponseHandler>();
            }, externalServiceCollection.BuildServiceProvider())
            .Build();
    }

    [Fact]
    public async Task Tycho_EnablesHandlingMessages_WithHandlerInstances()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut!.Publish<HandledByHandlerInstanceEvent>(new());
        await _sut.Execute<HandledByHandlerInstanceRequest>(new());
        var response = await _sut.Execute<HandledByHandlerInstanceRequestWithResponse, string>(new());

        // Assert
        Assert.True(EventWorkflowCompleted);
        Assert.True(RequestWorkflowCompleted);
        Assert.Equal(ReturnedResponse, response);
    }

    [Fact]
    public async Task Tycho_EnablesHandlingMessages_WithHandlerTypes()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut!.Publish<HandledByHandlerTypeEvent>(new());
        await _sut.Execute<HandledByHandlerTypeRequest>(new());
        var response = await _sut.Execute<HandledByHandlerTypeRequestWithResponse, string>(new());

        // Assert
        Assert.True(EventWorkflowCompleted);
        Assert.True(RequestWorkflowCompleted);
        Assert.Equal(ReturnedResponse, response);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
