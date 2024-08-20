using System;
using System.Collections.Generic;
using System.Linq;
using UnitTests.Utils;
using Tycho.Messaging;
using Tycho.Messaging.Handlers;

namespace UnitTests.Messaging;

public class MessageRouterTests
{
    private readonly MessageRouter _messageRouter;

    public MessageRouterTests()
    {
        _messageRouter = new MessageRouter();
        _messageRouter.RegisterEventHandler<OtherEvent>(new OtherMessageHandler());
        _messageRouter.RegisterRequestHandler<OtherCommand>(new OtherMessageHandler());
        _messageRouter.RegisterRequestWithResponseHandler<OtherQuery, object>(new OtherMessageHandler());
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
        IRequestHandler<TestCommand>? result = null;

        // Act
        Assert.Throws<KeyNotFoundException>(() => result = _messageRouter.GetRequestHandler<TestCommand>());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetCommandHandler_HandlerRegistered_ReturnsTheHandler()
    {
        // Arrange
        var registeredHandler = new TestMessageHandler();
        _messageRouter.RegisterRequestHandler<TestCommand>(registeredHandler);

        // Act
        var result = _messageRouter.GetRequestHandler<TestCommand>();

        // Assert
        Assert.Equal(registeredHandler, result);
    }

    [Fact]
    public void GetQueryHandler_NoHandler_ThrowsKeyNotFoundException()
    {
        // Arrange
        IRequestHandler<TestQuery, string>? result = null;

        // Act
        Assert.Throws<KeyNotFoundException>(() => result = _messageRouter.GetRequestWithResponseHandler<TestQuery, string>());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetQueryHandler_HandlerRegistered_ReturnsTheHandler()
    {
        // Arrange
        var registeredHandler = new TestMessageHandler();
        _messageRouter.RegisterRequestWithResponseHandler<TestQuery, string>(registeredHandler);

        // Act
        var result = _messageRouter.GetRequestWithResponseHandler<TestQuery, string>();

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
        Assert.Throws<ArgumentException>(() => _messageRouter.RegisterRequestHandler<TestCommand>(null!));

        // Assert
        Assert.Throws<KeyNotFoundException>(_messageRouter.GetRequestHandler<TestCommand>);
    }

    [Fact]
    public void RegisterCommandHandler_NoHandler_RegistersTheHandler()
    {
        // Arrange
        var handlerToRegister = new TestMessageHandler();

        // Act
        _messageRouter.RegisterRequestHandler<TestCommand>(handlerToRegister);

        // Assert
        Assert.Equal(handlerToRegister, _messageRouter.GetRequestHandler<TestCommand>());
    }

    [Fact]
    public void RegisterCommandHandler_AHandlerAlreadyRegistered_ThrowsArgumentException()
    {
        // Arrange
        var handlerToRegister = new TestMessageHandler();
        var registeredHandler = new OtherTestMessageHandler();
        _messageRouter.RegisterRequestHandler<TestCommand>(registeredHandler);

        // Act
        Assert.Throws<ArgumentException>(() => _messageRouter.RegisterRequestHandler<TestCommand>(handlerToRegister));

        // Assert
        Assert.Equal(registeredHandler, _messageRouter.GetRequestHandler<TestCommand>());
    }

    [Fact]
    public void RegisterQueryHandler_NullHandlerProvided_ThrowsArgumentException()
    {
        // Arrange
        // - no arrangement required

        // Act
        Assert.Throws<ArgumentException>(() => _messageRouter.RegisterRequestWithResponseHandler<TestQuery, string>(null!));

        // Assert
        Assert.Throws<KeyNotFoundException>(_messageRouter.GetRequestWithResponseHandler<TestQuery, string>);
    }

    [Fact]
    public void RegisterQueryHandler_NoHandler_RegistersTheHandler()
    {
        // Arrange
        var handlerToRegister = new TestMessageHandler();

        // Act
        _messageRouter.RegisterRequestWithResponseHandler<TestQuery, string>(handlerToRegister);

        // Assert
        Assert.Equal(handlerToRegister, _messageRouter.GetRequestWithResponseHandler<TestQuery, string>());
    }

    [Fact]
    public void RegisterQueryHandler_AHandlerAlreadyRegistered_ThrowsArgumentException()
    {
        // Arrange
        var handlerToRegister = new TestMessageHandler();
        var registeredHandler = new OtherTestMessageHandler();
        _messageRouter.RegisterRequestWithResponseHandler<TestQuery, string>(registeredHandler);

        // Act
        Assert.Throws<ArgumentException>(() => _messageRouter.RegisterRequestWithResponseHandler<TestQuery, string>(handlerToRegister));

        // Assert
        Assert.Equal(registeredHandler, _messageRouter.GetRequestWithResponseHandler<TestQuery, string>());
    }
}
