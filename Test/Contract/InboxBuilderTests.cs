using System.Threading;
using System.Threading.Tasks;
using Test.Utils;
using Tycho.Contract.Builders;
using Tycho.DependencyInjection;
using Tycho.Messaging;
using Tycho.Messaging.Handlers;

namespace Test.Contract;

public class InboxBuilderTests
{
    private readonly Mock<IMessageRouter> _messageRouterMock;
    private readonly InboxBuilder _inboxBuilder;

    public InboxBuilderTests()
    {
        var instanceCreatorMock = new Mock<IInstanceCreator>();
        _messageRouterMock = new Mock<IMessageRouter>();
        _inboxBuilder = new InboxBuilder(instanceCreatorMock.Object, _messageRouterMock.Object);
    }

    [Fact]
    public void PassesOn_RegistersEventHandler()
    {
        // Arrange
        // - no arrangement required

        // Act
        _inboxBuilder.PassesOn<TestEvent, TestModule>();
        _inboxBuilder.PassesOn<TestEvent, OtherEvent, TestModule>(eventData => new(int.MinValue));

        // Assert
        _messageRouterMock.Verify(router => router.RegisterEventHandler(It.IsAny<IEventHandler<TestEvent>>()), Times.Exactly(2));
    }

    [Fact]
    public void SubsribesTo_RegistersEventHandler()
    {
        // Arrange
        // - no arrangement required

        // Act
        _inboxBuilder.SubscribesTo((TestEvent _) => { });
        _inboxBuilder.SubscribesTo((TestEvent _) => Task.CompletedTask);
        _inboxBuilder.SubscribesTo((TestEvent _, CancellationToken _) => Task.CompletedTask);
        _inboxBuilder.SubscribesTo(new TestMessageHandler());
        _inboxBuilder.SubscribesTo<TestEvent, TestMessageHandler>();

        // Assert
        _messageRouterMock.Verify(router => router.RegisterEventHandler(It.IsAny<IEventHandler<TestEvent>>()), Times.Exactly(5));
    }

    [Fact]
    public void Forwards_RegistersCommandHandler()
    {
        // Arrange
        // - no arrangement required

        // Act
        _inboxBuilder.Forwards<TestCommand, TestModule>();
        _inboxBuilder.Forwards<TestCommand, OtherCommand, TestModule>(commandData => new(int.MinValue));

        // Assert
        _messageRouterMock.Verify(
            router => router.RegisterCommandHandler(It.IsAny<ICommandHandler<TestCommand>>()), 
            Times.Exactly(2));
    }

    [Fact]
    public void Executes_RegistersCommandHandler()
    {
        // Arrange
        // - no arrangement required

        // Act
        _inboxBuilder.Executes((TestCommand _) => { });
        _inboxBuilder.Executes((TestCommand _) => Task.CompletedTask);
        _inboxBuilder.Executes((TestCommand _, CancellationToken _) => Task.CompletedTask);
        _inboxBuilder.Executes(new TestMessageHandler());
        _inboxBuilder.Executes<TestCommand, TestMessageHandler>();

        // Assert
        _messageRouterMock.Verify(router => router.RegisterCommandHandler(It.IsAny<ICommandHandler<TestCommand>>()), Times.Exactly(5));
    }

    [Fact]
    public void Forwards_RegistersQueryHandler()
    {
        // Arrange
        // - no arrangement required

        // Act
        _inboxBuilder.Forwards<TestQuery, string, TestModule>();
        _inboxBuilder.Forwards<TestQuery, OtherTestQuery, string, TestModule>(queryData => new(queryData.Name));
        _inboxBuilder.Forwards<TestQuery, string, OtherQuery, object, TestModule>(
            queryData => new(int.MinValue), response => response.ToString());

        // Assert
        _messageRouterMock.Verify(
            router => router.RegisterQueryHandler(It.IsAny<IQueryHandler<TestQuery, string>>()), 
            Times.Exactly(3));
    }

    [Fact]
    public void RespondsTo_RegistersQueryHandler()
    {
        // Arrange
        var response = "test-response";

        // Act
        _inboxBuilder.RespondsTo((TestQuery _) => response);
        _inboxBuilder.RespondsTo((TestQuery _) => Task.FromResult(response));
        _inboxBuilder.RespondsTo((TestQuery _, CancellationToken _) => Task.FromResult(response));
        _inboxBuilder.RespondsTo(new TestMessageHandler());
        _inboxBuilder.RespondsTo<TestQuery, string, TestMessageHandler>();

        // Assert
        _messageRouterMock.Verify(router => router.RegisterQueryHandler(It.IsAny<IQueryHandler<TestQuery, string>>()), Times.Exactly(5));
    }

    [Fact]
    public async Task Build_ReturnsCorrectMessageBroker()
    {
        // Arrange
        var eventHandler = new TestMessageHandler();
        _messageRouterMock.Setup(router => router.GetEventHandlers<TestEvent>())
                          .Returns(new[] { eventHandler });

        var commandHandler = new TestMessageHandler();
        _messageRouterMock.Setup(router => router.GetCommandHandler<TestCommand>())
                          .Returns(commandHandler);

        var queryHandler = new TestMessageHandler();
        _messageRouterMock.Setup(router => router.GetQueryHandler<TestQuery, string>())
                          .Returns(queryHandler);

        // Act
        var broker = _inboxBuilder.Build();
        broker.Publish(new TestEvent("test-event"));
        await broker.Execute(new TestCommand("test-command"));
        var queryResponse = await broker.Execute<TestQuery, string>(new TestQuery("test-query"));

        // Assert
        Assert.True(eventHandler.HandlerCalled);
        Assert.True(commandHandler.HandlerCalled);
        Assert.True(queryHandler.HandlerCalled);
        Assert.Equal(queryHandler.QueryResponse, queryResponse);
    }
}
