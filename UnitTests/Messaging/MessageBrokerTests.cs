using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging;
using Tycho.Messaging.Handlers;
using UnitTests.Utils;

namespace UnitTests.Messaging;

public class MessageBrokerTests
{
    private readonly Mock<IEventHandler<TestEvent>> _eventHandlerMock;
    private readonly Mock<IEventHandler<TestEvent>> _otherEventHandlerMock;
    private readonly Mock<IRequestHandler<TestRequest>> _requestHandlerMock;
    private readonly Mock<IRequestHandler<TestRequestWithResponse, string>> _requestWithResponseHandlerMock;
    private readonly Mock<IMessageRouter> _messageRouterMock;
    private readonly MessageBroker _messageBroker;

    public MessageBrokerTests()
    {
        _eventHandlerMock = new Mock<IEventHandler<TestEvent>>();
        _otherEventHandlerMock = new Mock<IEventHandler<TestEvent>>();
        _requestHandlerMock = new Mock<IRequestHandler<TestRequest>>();
        _requestWithResponseHandlerMock = new Mock<IRequestHandler<TestRequestWithResponse, string>>();
        _messageRouterMock = new Mock<IMessageRouter>();
        _messageBroker = new MessageBroker(_messageRouterMock.Object);
    }

    [Fact]
    public void PublishEvent_NullEventData_ThrowsArgumentException()
    {
        // Arrange
        _messageRouterMock.Setup(router => router.GetEventHandlers<TestEvent>())
                          .Returns([_eventHandlerMock.Object, _otherEventHandlerMock.Object]);

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
                          .Returns([]);

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
                          .Returns([_eventHandlerMock.Object]);

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
                          .Returns([_eventHandlerMock.Object, _otherEventHandlerMock.Object]);

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
                          .Returns([_eventHandlerMock.Object]);

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
    public async Task ExecuteRequest_NullRequestData_ThrowsArgumentException()
    {
        // Arrange
        _messageRouterMock.Setup(router => router.GetRequestHandler<TestRequest>())
                          .Returns(_requestHandlerMock.Object);

        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _messageBroker.Execute<TestRequest>(null!, CancellationToken.None));

        // Assert
        _requestHandlerMock.Verify(handler => handler.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteRequest_NoHandler_ThrowsNullReferenceException()
    {
        // Arrange
        // - no arrangement required

        // Act
        await Assert.ThrowsAsync<NullReferenceException>(() => _messageBroker.Execute<TestRequest>(new TestRequest("test-request"), CancellationToken.None));

        // Assert
        _requestHandlerMock.Verify(handler => handler.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteRequest_HandlerPresent_CallsTheHandler()
    {
        // Arrange
        _messageRouterMock.Setup(router => router.GetRequestHandler<TestRequest>())
                          .Returns(_requestHandlerMock.Object);

        var requestToExecute = new TestRequest("test-request");
        var tokenToPass = new CancellationToken();

        // Act
        await _messageBroker.Execute(requestToExecute, tokenToPass);

        // Assert
        _requestHandlerMock.Verify(handler => handler.Handle(requestToExecute, tokenToPass), Times.Once());
    }

    [Fact]
    public async Task ExecuteRequest_WithLongRunningHandler_CompletesWithTheHandler()
    {
        // Arrange
        var handlerFinished = false;
        var handlerWorkload = async () =>
        {
            await Task.Delay(250);
            handlerFinished = true;
        };

        _requestHandlerMock.Setup(handler => handler.Handle(It.IsAny<TestRequest>(), default))
                           .Returns(handlerWorkload);

        _messageRouterMock.Setup(router => router.GetRequestHandler<TestRequest>())
                          .Returns(_requestHandlerMock.Object);

        var requestToExecute = new TestRequest("test-request");
        var tokenToPass = new CancellationToken();

        // Act
        await _messageBroker.Execute(requestToExecute, tokenToPass);

        // Assert
        Assert.True(handlerFinished);
        _requestHandlerMock.Verify(handler => handler.Handle(requestToExecute, tokenToPass), Times.Once());
    }

    [Fact]
    public async Task ExecuteRequestWithResponse_NullRequestData_ThrowsArgumentException()
    {
        // Arrange
        _messageRouterMock.Setup(router => router.GetRequestWithResponseHandler<TestRequestWithResponse, string>())
                          .Returns(_requestWithResponseHandlerMock.Object);

        // Act
        await Assert.ThrowsAsync<ArgumentException>(() => _messageBroker.Execute<TestRequestWithResponse, string>(null!, CancellationToken.None));

        // Assert
        _requestWithResponseHandlerMock.Verify(handler => handler.Handle(It.IsAny<TestRequestWithResponse>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteRequestWithResponse_NoHandler_ThrowsNullReferenceException()
    {
        // Arrange
        // - no arrangement required

        // Act
        await Assert.ThrowsAsync<NullReferenceException>(() => _messageBroker.Execute<TestRequestWithResponse, string>(new TestRequestWithResponse("test-request"), CancellationToken.None));

        // Assert
        _requestWithResponseHandlerMock.Verify(handler => handler.Handle(It.IsAny<TestRequestWithResponse>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteRequestWithResponse_HandlerPresent_ReturnsTheResult()
    {
        // Arrange
        var expectedResult = "result";

        _requestWithResponseHandlerMock.Setup(handler => handler.Handle(It.IsAny<TestRequestWithResponse>(), default))
                                       .ReturnsAsync(expectedResult);

        _messageRouterMock.Setup(router => router.GetRequestWithResponseHandler<TestRequestWithResponse, string>())
                          .Returns(_requestWithResponseHandlerMock.Object);

        var requestToExecute = new TestRequestWithResponse("test-request");
        var tokenToPass = new CancellationToken();

        // Act
        var result = await _messageBroker.Execute<TestRequestWithResponse, string>(requestToExecute, tokenToPass);

        // Assert
        Assert.Equal(expectedResult, result);
        _requestWithResponseHandlerMock.Verify(handler => handler.Handle(requestToExecute, tokenToPass), Times.Once());
    }

    [Fact]
    public async Task ExecuteRequestWithResponse_WithLongRunningHandler_CompletesWithTheHandler()
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

        _requestWithResponseHandlerMock.Setup(handler => handler.Handle(It.IsAny<TestRequestWithResponse>(), default))
                                       .Returns(handlerWorkload);

        _messageRouterMock.Setup(router => router.GetRequestWithResponseHandler<TestRequestWithResponse, string>())
                          .Returns(_requestWithResponseHandlerMock.Object);

        var requestToExecute = new TestRequestWithResponse("test-request");
        var tokenToPass = new CancellationToken();

        // Act
        var result = await _messageBroker.Execute<TestRequestWithResponse, string>(requestToExecute, tokenToPass);

        // Assert
        Assert.True(handlerFinished);
        Assert.Equal(expectedResult, result);
        _requestWithResponseHandlerMock.Verify(handler => handler.Handle(requestToExecute, tokenToPass), Times.Once());
    }
}
