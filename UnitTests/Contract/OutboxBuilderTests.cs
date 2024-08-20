using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tycho.Contract.Outbox;
using Tycho.Contract.Outbox.Builder;
using Tycho.DependencyInjection;
using Tycho.Messaging;
using Tycho.Messaging.Handlers;
using UnitTests.Utils;

namespace UnitTests.Contract;

public class OutboxBuilderTests
{
    private readonly OutboxBuilder _outboxBuilder;

    private IOutboxConsumer OutboxConsumer => _outboxBuilder;

    private IOutboxDefinition OutboxDefinition => _outboxBuilder;

    public OutboxBuilderTests()
    {
        var instanceCreatorMock = new Mock<IInstanceCreator>();
        var messageRouterMock = new MockMessageRouter();
        _outboxBuilder = new OutboxBuilder(instanceCreatorMock.Object, messageRouterMock);
    }

    [Fact]
    public void EventsDeclare_EventAlreadyDefined_ThrowsArgumentException()
    {
        // Arrange
        OutboxDefinition.Events.Declare<TestEvent>();

        // Act & Assert
        Assert.Throws<ArgumentException>(OutboxDefinition.Events.Declare<TestEvent>);
    }

    [Fact]
    public void RequestsDeclare_RequestAlreadyDefined_ThrowsArgumentException()
    {
        // Arrange
        OutboxDefinition.Requests.Declare<TestRequest>();

        // Act & Assert
        Assert.Throws<ArgumentException>(OutboxDefinition.Requests.Declare<TestRequest>);
    }

    [Fact]
    public void RequestsDeclareWithResponse_RequestAlreadyDefined_ThrowsArgumentException()
    {
        // Arrange
        OutboxDefinition.Requests.Declare<TestRequestWithResponse, string>();

        // Act & Assert
        Assert.Throws<ArgumentException>(OutboxDefinition.Requests.Declare<TestRequestWithResponse, string>);
    }

    [Fact]
    public void EventsForward_EventNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(OutboxConsumer.Events.Forward<TestEvent, TestModule>);
        Assert.Throws<InvalidOperationException>(() => OutboxConsumer.Events.Forward<TestEvent, OtherEvent, TestModule>(eventData => new(int.MinValue)));
    }

    [Fact]
    public void EventsExpose_EventNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(OutboxConsumer.Events.Expose<TestEvent>);
        Assert.Throws<InvalidOperationException>(() => OutboxConsumer.Events.Expose<TestEvent, OtherEvent>(eventData => new(int.MinValue)));
    }

    [Fact]
    public void EventsHandle_EventNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => OutboxConsumer.Events.Handle<TestEvent>(_ => { }));
    }

    [Fact]
    public void RequestsForward_RequestNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(OutboxConsumer.Requests.Forward<TestRequest, TestModule>);
        Assert.Throws<InvalidOperationException>(() => OutboxConsumer.Requests.Forward<TestRequest, OtherRequest, TestModule>(requestData => new(int.MinValue)));
    }

    [Fact]
    public void RequestsExpose_RequestNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(OutboxConsumer.Requests.Expose<TestRequest>);
        Assert.Throws<InvalidOperationException>(() => OutboxConsumer.Requests.Expose<TestRequest, OtherRequest>(requestData => new(int.MinValue)));
    }

    [Fact]
    public void RequestsHandle_RequestNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => OutboxConsumer.Requests.Handle<TestRequest>(_ => Task.CompletedTask));
    }

    [Fact]
    public void RequestsForwardWithResponse_RequestNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(OutboxConsumer.Requests.Forward<TestRequestWithResponse, string, TestModule>);
        Assert.Throws<InvalidOperationException>(
            () => OutboxConsumer.Requests.Forward<TestRequestWithResponse, string, OtherTestRequestWithResponse, string, TestModule>(
                requestData => new(requestData.Name), response => response));
        Assert.Throws<InvalidOperationException>(
            () => OutboxConsumer.Requests.Forward<TestRequestWithResponse, string, OtherRequestWithResponse, object, TestModule>(
                requestData => new(int.MinValue), response => response.ToString()!));
    }

    [Fact]
    public void RequestsExposeWithResponse_RequestNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(OutboxConsumer.Requests.Expose<TestRequestWithResponse, string>);
        Assert.Throws<InvalidOperationException>(
            () => OutboxConsumer.Requests.Expose<TestRequestWithResponse, string, OtherTestRequestWithResponse, string>(
                requestData => new(requestData.Name), response => response));
        Assert.Throws<InvalidOperationException>(
            () => OutboxConsumer.Requests.Expose<TestRequestWithResponse, string, OtherRequestWithResponse, object>(
                requestData => new(int.MinValue), response => response.ToString()!));
    }

    [Fact]
    public void RequestsHandleWithResponse_RequestNotDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(OutboxConsumer.Requests.Handle<TestRequestWithResponse, string, TestMessageHandler>);
    }

    [Fact]
    public void Build_AllHandlersMissing_ThrowsInvalidOperationException()
    {
        // Arrange
        OutboxDefinition.Events.Declare<TestEvent>();
        OutboxDefinition.Requests.Declare<TestRequest>();
        OutboxDefinition.Requests.Declare<TestRequestWithResponse, string>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(_outboxBuilder.Build);
    }

    [Fact]
    public void Build_RequestWithResponseHandlerMissing_ThrowsInvalidOperationException()
    {
        // Arrange
        OutboxDefinition.Events.Declare<TestEvent>();
        OutboxConsumer.Events.Handle<TestEvent>(_ => { });

        OutboxDefinition.Requests.Declare<TestRequest>();
        OutboxConsumer.Requests.Handle<TestRequest, TestMessageHandler>();

        OutboxDefinition.Requests.Declare<TestRequestWithResponse, string>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(_outboxBuilder.Build);
    }

    [Fact]
    public void Build_RequestsHandlerMissing_ThrowsInvalidOperationException()
    {
        // Arrange
        OutboxDefinition.Events.Declare<TestEvent>();
        OutboxConsumer.Events.Handle<TestEvent>((_, _) => Task.CompletedTask);

        OutboxDefinition.Requests.Declare<TestRequest>();

        OutboxDefinition.Requests.Declare<TestRequestWithResponse, string>();
        OutboxConsumer.Requests.Handle<TestRequestWithResponse, string>(_ => Task.FromResult("test-response"));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(_outboxBuilder.Build);
    }

    [Fact]
    public async Task Build_EventHandlerMissing_ReturnsCorrectMessageBroker()
    {
        // Arrange
        OutboxDefinition.Events.Declare<TestEvent>();

        var requestHandler = new TestMessageHandler();
        OutboxDefinition.Requests.Declare<TestRequest>();
        OutboxConsumer.Requests.Handle<TestRequest>(requestHandler);

        var requestWithResponseHandler = new TestMessageHandler();
        OutboxDefinition.Requests.Declare<TestRequestWithResponse, string>();
        OutboxConsumer.Requests.Handle<TestRequestWithResponse, string>(requestWithResponseHandler);

        // Act
        var broker = _outboxBuilder.Build();
        broker.Publish(new TestEvent("test-event"));
        await broker.Execute(new TestRequest("test-request"));
        var requestResponse = await broker.Execute<TestRequestWithResponse, string>(new TestRequestWithResponse("test-request"));

        // Assert
        Assert.True(requestHandler.HandlerCalled);
        Assert.True(requestWithResponseHandler.HandlerCalled);
        Assert.Equal(requestWithResponseHandler.RequestResponse, requestResponse);
    }

    [Fact]
    public async Task Build_AllMessagesHandled_ReturnsCorrectMessageBroker()
    {
        // Arrange
        var eventHandler = new TestMessageHandler();
        OutboxDefinition.Events.Declare<TestEvent>();
        OutboxConsumer.Events.Handle(eventHandler);

        var requestHandler = new TestMessageHandler();
        OutboxDefinition.Requests.Declare<TestRequest>();
        OutboxConsumer.Requests.Handle<TestRequest>(requestHandler);

        var requestWithResponseHandler = new TestMessageHandler();
        OutboxDefinition.Requests.Declare<TestRequestWithResponse, string>();
        OutboxConsumer.Requests.Handle<TestRequestWithResponse, string>(requestWithResponseHandler);

        // Act
        var broker = _outboxBuilder.Build();
        broker.Publish(new TestEvent("test-event"));
        await broker.Execute(new TestRequest("test-request"));
        var requestResponse = await broker.Execute<TestRequestWithResponse, string>(new TestRequestWithResponse("test-request"));

        // Assert
        Assert.True(eventHandler.HandlerCalled);
        Assert.True(requestHandler.HandlerCalled);
        Assert.True(requestWithResponseHandler.HandlerCalled);
        Assert.Equal(requestWithResponseHandler.RequestResponse, requestResponse);
    }

    [Fact]
    public void Build_NoMessagesDefined_ReturnsCorrectMessageBroker()
    {
        // Arrange
        // - no arrangement required

        // Act
        var broker = _outboxBuilder.Build();

        // Assert
        Assert.NotNull(broker);
    }

    private class MockMessageRouter : IMessageRouter
    {
        private readonly Dictionary<Type, object> _handlers = new();

        IEnumerable<IEventHandler<Event>> IMessageRouter.GetEventHandlers<Event>()
        {
            if (_handlers.ContainsKey(typeof(Event)))
            {
                return new[] { (_handlers[typeof(Event)] as IEventHandler<Event>)! };
            }
            return Array.Empty<IEventHandler<Event>>();
        }

        IRequestHandler<Request> IMessageRouter.GetRequestHandler<Request>() =>
            (_handlers[typeof(Request)] as IRequestHandler<Request>)!;

        IRequestHandler<Request, Response> IMessageRouter.GetRequestWithResponseHandler<Request, Response>() =>
            (_handlers[typeof(Request)] as IRequestHandler<Request, Response>)!;

        void IMessageRouter.RegisterEventHandler<Event>(IEventHandler<Event> eventHandler) =>
            _handlers[typeof(Event)] = eventHandler;

        void IMessageRouter.RegisterRequestHandler<Request>(IRequestHandler<Request> requestHandler) =>
            _handlers[typeof(Request)] = requestHandler;

        void IMessageRouter.RegisterRequestWithResponseHandler<Request, Response>(IRequestHandler<Request, Response> requestHandler) =>
            _handlers[typeof(Request)] = requestHandler;
    }
}
