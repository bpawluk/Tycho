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
    public bool CommandWorkflowCompleted { get; set; } = false;
    public string QueryResponse => "sample-response";

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

                consumer.Requests.Handle<HandledByLambdaCommand>(_ => CommandWorkflowCompleted = true)
                        .Requests.Handle<HandledByAsyncLambdaCommand>((_, _) =>
                        {
                            CommandWorkflowCompleted = true;
                            return Task.CompletedTask;
                        })
                        .Requests.Handle(new HandledByHandlerInstanceCommandHandler(this))
                        .Requests.Handle<HandledByHandlerTypeCommand, HandledByHandlerTypeCommandHandler>();

                consumer.Requests.Handle<HandledByLambdaQuery, string>(_ => QueryResponse)
                        .Requests.Handle<HandledByAsyncLambdaQuery, string>((_, _) =>
                        {
                            return Task.FromResult(QueryResponse);
                        })
                        .Requests.Handle(new HandledByHandlerInstanceQueryHandler(this))
                        .Requests.Handle<HandledByHandlerTypeQuery, string, HandledByHandlerTypeQueryHandler>();
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
        await _sut.Execute<HandledByLambdaCommand>(new());
        var queryResponse = await _sut.Execute<HandledByLambdaQuery, string>(new());

        // Assert
        Assert.True(EventWorkflowCompleted);
        Assert.True(CommandWorkflowCompleted);
        Assert.Equal(QueryResponse, queryResponse);

    }

    [Fact]
    public async Task Tycho_EnablesHandlingMessages_WithAsyncLambdas()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut!.Publish<HandledByAsyncLambdaEvent>(new());
        await _sut.Execute<HandledByAsyncLambdaCommand>(new());
        var queryResponse = await _sut.Execute<HandledByAsyncLambdaQuery, string>(new());

        // Assert
        Assert.True(EventWorkflowCompleted);
        Assert.True(CommandWorkflowCompleted);
        Assert.Equal(QueryResponse, queryResponse);
    }

    [Fact]
    public async Task Tycho_EnablesHandlingMessages_WithHandlerInstances()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut!.Publish<HandledByHandlerInstanceEvent>(new());
        await _sut.Execute<HandledByHandlerInstanceCommand>(new());
        var queryResponse = await _sut.Execute<HandledByHandlerInstanceQuery, string>(new());

        // Assert
        Assert.True(EventWorkflowCompleted);
        Assert.True(CommandWorkflowCompleted);
        Assert.Equal(QueryResponse, queryResponse);
    }

    [Fact]
    public async Task Tycho_EnablesHandlingMessages_WithHandlerTypes()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut!.Publish<HandledByHandlerTypeEvent>(new());
        await _sut.Execute<HandledByHandlerTypeCommand>(new());
        var queryResponse = await _sut.Execute<HandledByHandlerTypeQuery, string>(new());

        // Assert
        Assert.True(EventWorkflowCompleted);
        Assert.True(CommandWorkflowCompleted);
        Assert.Equal(QueryResponse, queryResponse);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
