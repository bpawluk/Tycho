using System;
using System.Collections.Generic;
using System.Linq;
using Tycho.Messaging;
using Tycho.Messaging.Handlers;
using UnitTests.Utils;

namespace UnitTests.Messaging;

public class MessageRouterTests
{
    private readonly MessageRouter _messageRouter;

    public MessageRouterTests()
    {
        _messageRouter = new MessageRouter();
        _messageRouter.RegisterEventHandler(new OtherMessageHandler());
        _messageRouter.RegisterRequestHandler(new OtherMessageHandler());
        _messageRouter.RegisterRequestWithResponseHandler(new OtherMessageHandler());
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
        _messageRouter.RegisterEventHandler(registeredHandler);

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
        registeredHandlers.ForEach(_messageRouter.RegisterEventHandler);

        // Act
        var result = _messageRouter.GetEventHandlers<TestEvent>();

        // Assert
        Assert.Equal(registeredHandlers, result);
    }

    [Fact]
    public void GetRequestHandler_NoHandler_ThrowsKeyNotFoundException()
    {
        // Arrange
        IRequestHandler<TestRequest>? result = null;

        // Act
        Assert.Throws<KeyNotFoundException>(() => result = _messageRouter.GetRequestHandler<TestRequest>());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetRequestHandler_HandlerRegistered_ReturnsTheHandler()
    {
        // Arrange
        var registeredHandler = new TestMessageHandler();
        _messageRouter.RegisterRequestHandler(registeredHandler);

        // Act
        var result = _messageRouter.GetRequestHandler<TestRequest>();

        // Assert
        Assert.Equal(registeredHandler, result);
    }

    [Fact]
    public void GetRequestWithResponseHandler_NoHandler_ThrowsKeyNotFoundException()
    {
        // Arrange
        IRequestHandler<TestRequestWithResponse, string>? result = null;

        // Act
        Assert.Throws<KeyNotFoundException>(() => result = _messageRouter.GetRequestWithResponseHandler<TestRequestWithResponse, string>());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetRequestWithResponseHandler_HandlerRegistered_ReturnsTheHandler()
    {
        // Arrange
        var registeredHandler = new TestMessageHandler();
        _messageRouter.RegisterRequestWithResponseHandler(registeredHandler);

        // Act
        var result = _messageRouter.GetRequestWithResponseHandler<TestRequestWithResponse, string>();

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
        _messageRouter.RegisterEventHandler(handlerToRegister);

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
        registeredHandlers.ForEach(_messageRouter.RegisterEventHandler);

        // Act
        _messageRouter.RegisterEventHandler(handlerToRegister);

        // Assert
        Assert.Equal(registeredHandlers.Append(handlerToRegister), _messageRouter.GetEventHandlers<TestEvent>());

    }

    [Fact]
    public void RegisterRequestHandler_NullHandlerProvided_ThrowsArgumentException()
    {
        // Arrange
        // - no arrangement required

        // Act
        Assert.Throws<ArgumentException>(() => _messageRouter.RegisterRequestHandler<TestRequest>(null!));

        // Assert
        Assert.Throws<KeyNotFoundException>(_messageRouter.GetRequestHandler<TestRequest>);
    }

    [Fact]
    public void RegisterRequestHandler_NoHandler_RegistersTheHandler()
    {
        // Arrange
        var handlerToRegister = new TestMessageHandler();

        // Act
        _messageRouter.RegisterRequestHandler(handlerToRegister);

        // Assert
        Assert.Equal(handlerToRegister, _messageRouter.GetRequestHandler<TestRequest>());
    }

    [Fact]
    public void RegisterRequestHandler_AHandlerAlreadyRegistered_ThrowsArgumentException()
    {
        // Arrange
        var handlerToRegister = new TestMessageHandler();
        var registeredHandler = new OtherTestMessageHandler();
        _messageRouter.RegisterRequestHandler(registeredHandler);

        // Act
        Assert.Throws<ArgumentException>(() => _messageRouter.RegisterRequestHandler(handlerToRegister));

        // Assert
        Assert.Equal(registeredHandler, _messageRouter.GetRequestHandler<TestRequest>());
    }

    [Fact]
    public void RegisterRequestWithResponseHandler_NullHandlerProvided_ThrowsArgumentException()
    {
        // Arrange
        // - no arrangement required

        // Act
        Assert.Throws<ArgumentException>(() => _messageRouter.RegisterRequestWithResponseHandler<TestRequestWithResponse, string>(null!));

        // Assert
        Assert.Throws<KeyNotFoundException>(_messageRouter.GetRequestWithResponseHandler<TestRequestWithResponse, string>);
    }

    [Fact]
    public void RegisterRequestWithResponseHandler_NoHandler_RegistersTheHandler()
    {
        // Arrange
        var handlerToRegister = new TestMessageHandler();

        // Act
        _messageRouter.RegisterRequestWithResponseHandler(handlerToRegister);

        // Assert
        Assert.Equal(handlerToRegister, _messageRouter.GetRequestWithResponseHandler<TestRequestWithResponse, string>());
    }

    [Fact]
    public void RegisterRequestWithResponseHandler_AHandlerAlreadyRegistered_ThrowsArgumentException()
    {
        // Arrange
        var handlerToRegister = new TestMessageHandler();
        var registeredHandler = new OtherTestMessageHandler();
        _messageRouter.RegisterRequestWithResponseHandler(registeredHandler);

        // Act
        Assert.Throws<ArgumentException>(() => _messageRouter.RegisterRequestWithResponseHandler(handlerToRegister));

        // Assert
        Assert.Equal(registeredHandler, _messageRouter.GetRequestWithResponseHandler<TestRequestWithResponse, string>());
    }
}
