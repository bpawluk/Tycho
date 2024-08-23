using System.Threading.Tasks;
using Tycho.Contract.Inbox.Builder;
using Tycho.DependencyInjection;
using Tycho.Messaging;
using Tycho.Messaging.Handlers;
using UnitTests.Utils;

namespace UnitTests.Contract;

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
    public void EventsForward_RegistersEventHandler()
    {
        // Arrange
        // - no arrangement required

        // Act
        _inboxBuilder.Events.Forward<TestEvent, TestModule>();
        _inboxBuilder.Events.Forward<TestEvent, OtherEvent, TestModule>(eventData => new(int.MinValue));

        // Assert
        _messageRouterMock.Verify(router => router.RegisterEventHandler(It.IsAny<IEventHandler<TestEvent>>()), Times.Exactly(2));
    }

    [Fact]
    public void EventsHandle_RegistersEventHandler()
    {
        // Arrange
        // - no arrangement required

        // Act
        _inboxBuilder.Events.Handle(new TestMessageHandler());
        _inboxBuilder.Events.Handle<TestEvent, TestMessageHandler>();

        // Assert
        _messageRouterMock.Verify(router => router.RegisterEventHandler(It.IsAny<IEventHandler<TestEvent>>()), Times.Exactly(2));
    }

    [Fact]
    public void RequestsForward_RegistersRequestHandler()
    {
        // Arrange
        // - no arrangement required

        // Act
        _inboxBuilder.Requests.Forward<TestRequest, TestModule>();
        _inboxBuilder.Requests.Forward<TestRequest, OtherRequest, TestModule>(requestData => new(int.MinValue));

        // Assert
        _messageRouterMock.Verify(
            router => router.RegisterRequestHandler(It.IsAny<IRequestHandler<TestRequest>>()), 
            Times.Exactly(2));
    }

    [Fact]
    public void RequestsHandle_RegistersRequestHandler()
    {
        // Arrange
        // - no arrangement required

        // Act
        _inboxBuilder.Requests.Handle<TestRequest>(new TestMessageHandler());
        _inboxBuilder.Requests.Handle<TestRequest, TestMessageHandler>();

        // Assert
        _messageRouterMock.Verify(router => router.RegisterRequestHandler(It.IsAny<IRequestHandler<TestRequest>>()), Times.Exactly(2));
    }

    [Fact]
    public void RequestsForwardWithResponse_RegistersRequestHandler()
    {
        // Arrange
        // - no arrangement required

        // Act
        _inboxBuilder.Requests.Forward<TestRequestWithResponse, string, TestModule>();
        _inboxBuilder.Requests.Forward<TestRequestWithResponse, string, OtherTestRequestWithResponse, string, TestModule>(requestData => new(requestData.Name), response => response);
        _inboxBuilder.Requests.Forward<TestRequestWithResponse, string, OtherRequestWithResponse, object, TestModule>(
            requestData => new(int.MinValue), response => response.ToString()!);

        // Assert
        _messageRouterMock.Verify(
            router => router.RegisterRequestWithResponseHandler(It.IsAny<IRequestHandler<TestRequestWithResponse, string>>()), 
            Times.Exactly(3));
    }

    [Fact]
    public void RequestsHandleWithResponse_RegistersRequestHandler()
    {
        // Arrange
        // - no arrangement required

        // Act
        _inboxBuilder.Requests.Handle<TestRequestWithResponse, string>(new TestMessageHandler());
        _inboxBuilder.Requests.Handle<TestRequestWithResponse, string, TestMessageHandler>();

        // Assert
        _messageRouterMock.Verify(router => router.RegisterRequestWithResponseHandler(It.IsAny<IRequestHandler<TestRequestWithResponse, string>>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Build_ReturnsCorrectMessageBroker()
    {
        // Arrange
        var eventHandler = new TestMessageHandler();
        _messageRouterMock.Setup(router => router.GetEventHandlers<TestEvent>())
                          .Returns([eventHandler]);

        var requestHandler = new TestMessageHandler();
        _messageRouterMock.Setup(router => router.GetRequestHandler<TestRequest>())
                          .Returns(requestHandler);

        var requestWithResponseHandler = new TestMessageHandler();
        _messageRouterMock.Setup(router => router.GetRequestWithResponseHandler<TestRequestWithResponse, string>())
                          .Returns(requestWithResponseHandler);

        // Act
        var broker = _inboxBuilder.Build();
        broker.Publish(new TestEvent("test-event"));
        await broker.Execute(new TestRequest("test-request"));
        var requestResponse = await broker.Execute<TestRequestWithResponse, string>(new TestRequestWithResponse("test-request"));

        // Assert
        Assert.True(eventHandler.HandlerCalled);
        Assert.True(requestHandler.HandlerCalled);
        Assert.True(requestWithResponseHandler.HandlerCalled);
        Assert.Equal(requestHandler.RequestResponse, requestResponse);
    }
}
