using System;
using System.Collections.Generic;
using System.Linq;
using Test.Utils;
using Tycho.Messaging;
using Tycho.Messaging.Handlers;

namespace Test.Messaging;

public class MessageRouterTests
{
    private readonly MessageRouter _messageRouter;

    public MessageRouterTests()
    {
        _messageRouter = new MessageRouter();
        _messageRouter.RegisterEventHandler<OtherEvent>(new OtherMessageHandler());
        _messageRouter.RegisterCommandHandler<OtherCommand>(new OtherMessageHandler());
        _messageRouter.RegisterQueryHandler<OtherQuery, object>(new OtherMessageHandler());
    }

    [Fact]
    public void GetEventHandlers_NoHandlers_ReturnsEmptyEnumerable()
    {
        // Arrange
        // - no arrangement required

        // Act
        var result = _messageRouter.GetEventHandlers<TestEvent>();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetEventHandlers_SingleHandlerRegistered_ReturnsSingleEnumerable()
    {
        // Arrange
        var registeredHandler = new TestMessageHandler();
        _messageRouter.RegisterEventHandler<TestEvent>(registeredHandler);

        // Act
        var result = _messageRouter.GetEventHandlers<TestEvent>();

        // Assert
        Assert.Single(result, registeredHandler);
    }

    [Fact]
    public void GetEventHandlers_MultipleHandlersRegistered_ReturnsAllHandlers()
    {
        // Arrange
        var registeredHandlers = new List<IEventHandler<TestEvent>>
        {
            new TestMessageHandler(),
            new OtherTestMessageHandler(),
            new YetAnotherTestMessageHandler()
        };
        registeredHandlers.ForEach(_messageRouter.RegisterEventHandler<TestEvent>);

        // Act
        var result = _messageRouter.GetEventHandlers<TestEvent>();

        // Assert
        Assert.Equal(registeredHandlers, result);
    }

    [Fact]
    public void GetCommandHandler_NoHandler_ThrowsKeyNotFoundException()
    {
        // Arrange
        ICommandHandler<TestCommand>? result = null;

        // Act
        Assert.Throws<KeyNotFoundException>(() => result = _messageRouter.GetCommandHandler<TestCommand>());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetCommandHandler_HandlerRegistered_ReturnsTheHandler()
    {
        // Arrange
        var registeredHandler = new TestMessageHandler();
        _messageRouter.RegisterCommandHandler<TestCommand>(registeredHandler);

        // Act
        var result = _messageRouter.GetCommandHandler<TestCommand>();

        // Assert
        Assert.Equal(registeredHandler, result);
    }

    [Fact]
    public void GetQueryHandler_NoHandler_ThrowsKeyNotFoundException()
    {
        // Arrange
        IQueryHandler<TestQuery, string>? result = null;

        // Act
        Assert.Throws<KeyNotFoundException>(() => result = _messageRouter.GetQueryHandler<TestQuery, string>());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetQueryHandler_HandlerRegistered_ReturnsTheHandler()
    {
        // Arrange
        var registeredHandler = new TestMessageHandler();
        _messageRouter.RegisterQueryHandler<TestQuery, string>(registeredHandler);

        // Act
        var result = _messageRouter.GetQueryHandler<TestQuery, string>();

        // Assert
        Assert.Equal(registeredHandler, result);
    }

    [Fact]
    public void RegisterEventHandler_NullHandlerProvided_ThrowsArgumentException()
    {
        // Arrange
        // - no arrangement required

        // Act
        Assert.Throws<ArgumentException>(() => _messageRouter.RegisterEventHandler<TestEvent>(null!));

        // Assert
        Assert.Empty(_messageRouter.GetEventHandlers<TestEvent>());
    }

    [Fact]
    public void RegisterEventHandler_NoOtherHandlers_RegistersTheHandler()
    {
        // Arrange
        var handlerToRegister = new TestMessageHandler();

        // Act
        _messageRouter.RegisterEventHandler<TestEvent>(handlerToRegister);

        // Assert
        Assert.Single(_messageRouter.GetEventHandlers<TestEvent>(), handlerToRegister);
    }

    [Fact]
    public void RegisterEventHandler_OtherHandlersRegistered_RegistersTheHandler()
    {
        // Arrange
        var handlerToRegister = new TestMessageHandler();
        var registeredHandlers = new List<IEventHandler<TestEvent>>
        {
            new OtherTestMessageHandler(),
            new YetAnotherTestMessageHandler()
        };
        registeredHandlers.ForEach(_messageRouter.RegisterEventHandler<TestEvent>);

        // Act
        _messageRouter.RegisterEventHandler<TestEvent>(handlerToRegister);

        // Assert
        Assert.Equal(registeredHandlers.Append(handlerToRegister), _messageRouter.GetEventHandlers<TestEvent>());

    }

    [Fact]
    public void RegisterCommandHandler_NullHandlerProvided_ThrowsArgumentException()
    {
        // Arrange
        // - no arrangement required

        // Act
        Assert.Throws<ArgumentException>(() => _messageRouter.RegisterCommandHandler<TestCommand>(null!));

        // Assert
        Assert.Throws<KeyNotFoundException>(_messageRouter.GetCommandHandler<TestCommand>);
    }

    [Fact]
    public void RegisterCommandHandler_NoHandler_RegistersTheHandler()
    {
        // Arrange
        var handlerToRegister = new TestMessageHandler();

        // Act
        _messageRouter.RegisterCommandHandler<TestCommand>(handlerToRegister);

        // Assert
        Assert.Equal(handlerToRegister, _messageRouter.GetCommandHandler<TestCommand>());
    }

    [Fact]
    public void RegisterCommandHandler_AHandlerAlreadyRegistered_ThrowsArgumentException()
    {
        // Arrange
        var handlerToRegister = new TestMessageHandler();
        var registeredHandler = new OtherTestMessageHandler();
        _messageRouter.RegisterCommandHandler<TestCommand>(registeredHandler);

        // Act
        Assert.Throws<ArgumentException>(() => _messageRouter.RegisterCommandHandler<TestCommand>(handlerToRegister));

        // Assert
        Assert.Equal(registeredHandler, _messageRouter.GetCommandHandler<TestCommand>());
    }

    [Fact]
    public void RegisterQueryHandler_NullHandlerProvided_ThrowsArgumentException()
    {
        // Arrange
        // - no arrangement required

        // Act
        Assert.Throws<ArgumentException>(() => _messageRouter.RegisterQueryHandler<TestQuery, string>(null!));

        // Assert
        Assert.Throws<KeyNotFoundException>(_messageRouter.GetQueryHandler<TestQuery, string>);
    }

    [Fact]
    public void RegisterQueryHandler_NoHandler_RegistersTheHandler()
    {
        // Arrange
        var handlerToRegister = new TestMessageHandler();

        // Act
        _messageRouter.RegisterQueryHandler<TestQuery, string>(handlerToRegister);

        // Assert
        Assert.Equal(handlerToRegister, _messageRouter.GetQueryHandler<TestQuery, string>());
    }

    [Fact]
    public void RegisterQueryHandler_AHandlerAlreadyRegistered_ThrowsArgumentException()
    {
        // Arrange
        var handlerToRegister = new TestMessageHandler();
        var registeredHandler = new OtherTestMessageHandler();
        _messageRouter.RegisterQueryHandler<TestQuery, string>(registeredHandler);

        // Act
        Assert.Throws<ArgumentException>(() => _messageRouter.RegisterQueryHandler<TestQuery, string>(handlerToRegister));

        // Assert
        Assert.Equal(registeredHandler, _messageRouter.GetQueryHandler<TestQuery, string>());
    }
}
