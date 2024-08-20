using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Forwarders;
using Tycho.Messaging.Interceptors;
using UnitTests.Utils;

namespace UnitTests.Messaging.Forwarders;

public class DownForwarderTests
{
    [Fact]
    public async Task EventDownForwarder_PassesEventToAnotherModule()
    {
        // Arrange
        var moduleMock = new Mock<IModule<TestModule>>();
        var interceptorMock = new Mock<IEventInterceptor<OtherEvent>>();
        var expectedEvent = new OtherEvent(int.MinValue);
        var expectedToken = new CancellationToken();
        var handler = new EventDownForwarder<TestEvent, OtherEvent, TestModule>(moduleMock.Object, _ => expectedEvent, interceptorMock.Object);

        // Act
        await handler.Handle(new TestEvent("event-name"), expectedToken);

        // Assert
        moduleMock.Verify(module => module.Publish(expectedEvent, expectedToken), Times.Once);
        interceptorMock.Verify(module => module.ExecuteBefore(expectedEvent, expectedToken), Times.Once);
        interceptorMock.Verify(module => module.ExecuteAfter(expectedEvent, expectedToken), Times.Once);
    }

    [Fact]
    public async Task RequestDownForwarder_ForwardsRequestToAnotherModule()
    {
        // Arrange
        var moduleMock = new Mock<IModule<TestModule>>();
        var interceptorMock = new Mock<IRequestInterceptor<OtherRequest>>();
        var expectedRequest = new OtherRequest(int.MinValue);
        var expectedToken = new CancellationToken();
        var handler = new RequestDownForwarder<TestRequest, OtherRequest, TestModule>(moduleMock.Object, _ => expectedRequest, interceptorMock.Object);

        // Act
        await handler.Handle(new TestRequest("request-name"), expectedToken);

        // Assert
        moduleMock.Verify(module => module.Execute(expectedRequest, expectedToken), Times.Once);
        interceptorMock.Verify(module => module.ExecuteBefore(expectedRequest, expectedToken), Times.Once);
        interceptorMock.Verify(module => module.ExecuteAfter(expectedRequest, expectedToken), Times.Once);
    }

    [Fact]
    public async Task RequestWithResponseDownForwarder_ForwardsRequestToAnotherModule()
    {
        // Arrange
        var moduleResponse = new object();
        var moduleMock = new Mock<IModule<TestModule>>();
        moduleMock.Setup(module => module.Execute<OtherRequestWithResponse, object>(It.IsAny<OtherRequestWithResponse>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(moduleResponse);
        var interceptorMock = new Mock<IRequestInterceptor<OtherRequestWithResponse, object>>();
        interceptorMock.Setup(interceptor => interceptor.ExecuteAfter(It.IsAny<OtherRequestWithResponse>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync((OtherRequestWithResponse  request, object result, CancellationToken token) => result);

        var expectedRequest = new OtherRequestWithResponse(int.MinValue);
        var expectedToken = new CancellationToken();
        var handler = new RequestDownForwarder<TestRequestWithResponse, string, OtherRequestWithResponse, object, TestModule>(moduleMock.Object, _ => expectedRequest, response => response.ToString()!, interceptorMock.Object);

        // Act
        var result = await handler.Handle(new TestRequestWithResponse("request-with-response-name"), expectedToken);

        // Assert
        moduleMock.Verify(module => module.Execute<OtherRequestWithResponse, object>(expectedRequest, expectedToken), Times.Once);
        interceptorMock.Verify(module => module.ExecuteBefore(expectedRequest, expectedToken), Times.Once);
        interceptorMock.Verify(module => module.ExecuteAfter(expectedRequest, moduleResponse, expectedToken), Times.Once);
        Assert.Equal(moduleResponse.ToString(), result);
    }
}
