using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Test.Utils;
using Tycho.Messaging;
using Tycho.Messaging.Handlers;

namespace Test.Messaging;

public class MessageBrokerTests
{
    private readonly Mock<IEventHandler<TestEvent>> _eventHandlerMock;
    private readonly Mock<IEventHandler<TestEvent>> _otherEventHandlerMock;
    private readonly Mock<ICommandHandler<TestCommand>> _commandHandlerMock;
    private readonly Mock<IQueryHandler<TestQuery, string>> _queryHandlerMock;
    private readonly Mock<IMessageRouter> _messageRouterMock;
    private readonly MessageBroker _messageBroker;

    public MessageBrokerTests()
    {
        _eventHandlerMock = new Mock<IEventHandler<TestEvent>>();
        _otherEventHandlerMock = new Mock<IEventHandler<TestEvent>>();
        _commandHandlerMock = new Mock<ICommandHandler<TestCommand>>();
        _queryHandlerMock = new Mock<IQueryHandler<TestQuery, string>>();
        _messageRouterMock = new Mock<IMessageRouter>();
        _messageBroker = new MessageBroker(_messageRouterMock.Object);
    }

    [Fact]
    public void PublishEvent_NullEventData_ThrowsArgumentException()
    {
        // Arrange
        _messageRouterMock.Setup(router => router.GetEventHandlers<TestEvent>())
                          .Returns(new[] { _eventHandlerMock.Object, _otherEventHandlerMock.Object });

        // Act
        Assert.Throws<ArgumentException>(() => _messageBroker.Publish<TestEvent>(null!, CancellationToken.None));

        // Assert
        _eventHandlerMock.Verify(handler => handler.Handle(It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()), Times.Never());
        _otherEventHandlerMock.Verify(handler => handler.Handle(It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public void PublishEvent_NoHandlers_DoesNotThrow()
    {
        // Arrange
        _messageRouterMock.Setup(router => router.GetEventHandlers<TestEvent>())
                          .Returns(Enumerable.Empty<IEventHandler<TestEvent>>());

        // Act
        _messageBroker.Publish(new TestEvent("test-event"), CancellationToken.None);

        // Assert
        _eventHandlerMock.Verify(handler => handler.Handle(It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()), Times.Never());
        _otherEventHandlerMock.Verify(handler => handler.Handle(It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public void PublishEvent_SingleHandler_CallsTheHandler()
    {
        // Arrange
        _messageRouterMock.Setup(router => router.GetEventHandlers<TestEvent>())
                          .Returns(new[] { _eventHandlerMock.Object });

        var eventToPublish = new TestEvent("test-event");
        var tokenToPass = new CancellationToken();

        // Act
        _messageBroker.Publish(eventToPublish, tokenToPass);

        // Assert
        _eventHandlerMock.Verify(handler => handler.Handle(eventToPublish, tokenToPass), Times.Once());
        _otherEventHandlerMock.Verify(handler => handler.Handle(It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public void PublishEvent_MultipleHandlers_CallsAllOfThem()
    {
        // Arrange
        _messageRouterMock.Setup(router => router.GetEventHandlers<TestEvent>())
                          .Returns(new[] { _eventHandlerMock.Object, _otherEventHandlerMock.Object });

        var eventToPublish = new TestEvent("test-event");
        var tokenToPass = new CancellationToken();

        // Act
        _messageBroker.Publish(eventToPublish, tokenToPass);

        // Assert
        _eventHandlerMock.Verify(handler => handler.Handle(eventToPublish, tokenToPass), Times.Once());
        _otherEventHandlerMock.Verify(handler => handler.Handle(eventToPublish, tokenToPass), Times.Once());
    }

    [Fact]
    public void PublishEvent_WithLongRunningHandler_CompletesImmediately()
    {
        // Arrange
        var handlerFinished = false;
        var handlerWorkload = async () =>
        {
            await Task.Delay(250);
            handlerFinished = true;
        };

        _eventHandlerMock.Setup(handler => handler.Handle(It.IsAny<TestEvent>(), default))
                         .Returns(handlerWorkload);

        _messageRouterMock.Setup(router => router.GetEventHandlers<TestEvent>())
                          .Returns(new[] { _eventHandlerMock.Object });

        var eventToPublish = new TestEvent("test-event");
        var tokenToPass = new CancellationToken();

        // Act
        _messageBroker.Publish(eventToPublish, tokenToPass);

        // Assert
        Assert.False(handlerFinished);
        _eventHandlerMock.Verify(handler => handler.Handle(eventToPublish, tokenToPass), Times.Once());
        _otherEventHandlerMock.Verify(handler => handler.Handle(It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteCommand_NullCommandData_ThrowsArgumentException()
    {
        // Arrange
        _messageRouterMock.Setup(router => router.GetCommandHandler<TestCommand>())
                          .Returns(_commandHandlerMock.Object);

        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _messageBroker.Execute<TestCommand>(null!, CancellationToken.None));

        // Assert
        _commandHandlerMock.Verify(handler => handler.Handle(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteCommand_NoHandler_ThrowsNullReferenceException()
    {
        // Arrange
        // - no arrangement required

        // Act
        await Assert.ThrowsAsync<NullReferenceException>(() => _messageBroker.Execute<TestCommand>(new TestCommand("test-command"), CancellationToken.None));

        // Assert
        _commandHandlerMock.Verify(handler => handler.Handle(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteCommand_HandlerPresent_CallsTheHandler()
    {
        // Arrange
        _messageRouterMock.Setup(router => router.GetCommandHandler<TestCommand>())
                          .Returns(_commandHandlerMock.Object);

        var commandToExecute = new TestCommand("test-command");
        var tokenToPass = new CancellationToken();

        // Act
        await _messageBroker.Execute(commandToExecute, tokenToPass);

        // Assert
        _commandHandlerMock.Verify(handler => handler.Handle(commandToExecute, tokenToPass), Times.Once());
    }

    [Fact]
    public async Task ExecuteCommand_WithLongRunningHandler_CompletesWithTheHandler()
    {
        // Arrange
        var handlerFinished = false;
        var handlerWorkload = async () =>
        {
            await Task.Delay(250);
            handlerFinished = true;
        };

        _commandHandlerMock.Setup(handler => handler.Handle(It.IsAny<TestCommand>(), default))
                           .Returns(handlerWorkload);

        _messageRouterMock.Setup(router => router.GetCommandHandler<TestCommand>())
                          .Returns(_commandHandlerMock.Object);

        var commandToExecute = new TestCommand("test-command");
        var tokenToPass = new CancellationToken();

        // Act
        await _messageBroker.Execute(commandToExecute, tokenToPass);

        // Assert
        Assert.True(handlerFinished);
        _commandHandlerMock.Verify(handler => handler.Handle(commandToExecute, tokenToPass), Times.Once());
    }

    [Fact]
    public async Task ExecuteQuery_NullQueryData_ThrowsArgumentException()
    {
        // Arrange
        _messageRouterMock.Setup(router => router.GetQueryHandler<TestQuery, string>())
                          .Returns(_queryHandlerMock.Object);

        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _messageBroker.Execute<TestQuery, string>(null!, CancellationToken.None));

        // Assert
        _queryHandlerMock.Verify(handler => handler.Handle(It.IsAny<TestQuery>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteQuery_NoHandler_ThrowsNullReferenceException()
    {
        // Arrange
        // - no arrangement required

        // Act
        await Assert.ThrowsAsync<NullReferenceException>(() => _messageBroker.Execute<TestQuery, string>(new TestQuery("test-query"), CancellationToken.None));

        // Assert
        _queryHandlerMock.Verify(handler => handler.Handle(It.IsAny<TestQuery>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteQuery_HandlerPresent_ReturnsTheResult()
    {
        // Arrange
        var expectedResult = "result";

        _queryHandlerMock.Setup(handler => handler.Handle(It.IsAny<TestQuery>(), default))
                         .ReturnsAsync(expectedResult);

        _messageRouterMock.Setup(router => router.GetQueryHandler<TestQuery, string>())
                          .Returns(_queryHandlerMock.Object);

        var queryToExecute = new TestQuery("test-query");
        var tokenToPass = new CancellationToken();

        // Act
        var result = await _messageBroker.Execute<TestQuery, string>(queryToExecute, tokenToPass);

        // Assert
        Assert.Equal(expectedResult, result);
        _queryHandlerMock.Verify(handler => handler.Handle(queryToExecute, tokenToPass), Times.Once());
    }

    [Fact]
    public async Task ExecuteQuery_WithLongRunningHandler_CompletesWithTheHandler()
    {
        // Arrange
        var expectedResult = "result";

        var handlerFinished = false;
        var handlerWorkload = async () =>
        {
            await Task.Delay(250);
            handlerFinished = true;
            return expectedResult;
        };

        _queryHandlerMock.Setup(handler => handler.Handle(It.IsAny<TestQuery>(), default))
                         .Returns(handlerWorkload);

        _messageRouterMock.Setup(router => router.GetQueryHandler<TestQuery, string>())
                          .Returns(_queryHandlerMock.Object);

        var queryToExecute = new TestQuery("test-query");
        var tokenToPass = new CancellationToken();

        // Act
        var result = await _messageBroker.Execute<TestQuery, string>(queryToExecute, tokenToPass);

        // Assert
        Assert.True(handlerFinished);
        Assert.Equal(expectedResult, result);
        _queryHandlerMock.Verify(handler => handler.Handle(queryToExecute, tokenToPass), Times.Once());
    }
}
