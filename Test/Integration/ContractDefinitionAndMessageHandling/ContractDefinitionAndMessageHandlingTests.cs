using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Test.Integration.ContractDefinitionAndMessageHandling.SUT;
using Test.Integration.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;
using Tycho;

namespace Test.Integration.ContractDefinitionAndMessageHandling;

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
                consumer.HandleEvent<HandledByLambdaEvent>(_ => EventWorkflowCompleted = true)
                        .HandleEvent<HandledByAsyncLambdaEvent>((_, _) =>
                        {
                            EventWorkflowCompleted = true;
                            return Task.CompletedTask;
                        })
                        .HandleEvent(new HandledByHandlerInstanceEventHandler(this))
                        .HandleEvent<HandledByHandlerTypeEvent, HandledByHandlerTypeEventHandler>();

                consumer.HandleCommand<HandledByLambdaCommand>(_ => CommandWorkflowCompleted = true)
                        .HandleCommand<HandledByAsyncLambdaCommand>((_, _) =>
                        {
                            CommandWorkflowCompleted = true;
                            return Task.CompletedTask;
                        })
                        .HandleCommand(new HandledByHandlerInstanceCommandHandler(this))
                        .HandleCommand<HandledByHandlerTypeCommand, HandledByHandlerTypeCommandHandler>();

                consumer.HandleQuery<HandledByLambdaQuery, string>(_ => QueryResponse)
                        .HandleQuery<HandledByAsyncLambdaQuery, string>((_, _) =>
                        {
                            return Task.FromResult(QueryResponse);
                        })
                        .HandleQuery(new HandledByHandlerInstanceQueryHandler(this))
                        .HandleQuery<HandledByHandlerTypeQuery, string, HandledByHandlerTypeQueryHandler>();
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
