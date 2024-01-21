using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Forwarders;
using Tycho.Messaging.Interceptors;
using UnitTests.Utils;

namespace UnitTests.Messaging.Forwarders;

public class UpForwarderTests
{
    [Fact]
    public async Task EventUpForwarder_PassesEventOutsideTheModule()
    {
        // Arrange
        var moduleMock = new Mock<IModule>();
        var interceptorMock = new Mock<IEventInterceptor<OtherEvent>>();
        var expectedEvent = new OtherEvent(int.MinValue);
        var expectedToken = new CancellationToken();
        var handler = new EventUpForwarder<TestEvent, OtherEvent>(moduleMock.Object, _ => expectedEvent, interceptorMock.Object);

        // Act
        await handler.Handle(new TestEvent("event-name"), expectedToken);

        // Assert
        moduleMock.Verify(module => module.Publish(expectedEvent, expectedToken), Times.Once);
        interceptorMock.Verify(module => module.ExecuteBefore(expectedEvent, expectedToken), Times.Once);
        interceptorMock.Verify(module => module.ExecuteAfter(expectedEvent, expectedToken), Times.Once);
    }

    [Fact]
    public async Task CommandUpForwarder_ForwardsCommandOutsideTheModule()
    {
        // Arrange
        var moduleMock = new Mock<IModule>();
        var interceptorMock = new Mock<ICommandInterceptor<OtherCommand>>();
        var expectedCommand = new OtherCommand(int.MinValue);
        var expectedToken = new CancellationToken();
        var handler = new CommandUpForwarder<TestCommand, OtherCommand>(moduleMock.Object, _ => expectedCommand, interceptorMock.Object);

        // Act
        await handler.Handle(new TestCommand("command-name"), expectedToken);

        // Assert
        moduleMock.Verify(module => module.Execute(expectedCommand, expectedToken), Times.Once);
        interceptorMock.Verify(module => module.ExecuteBefore(expectedCommand, expectedToken), Times.Once);
        interceptorMock.Verify(module => module.ExecuteAfter(expectedCommand, expectedToken), Times.Once);
    }

    [Fact]
    public async Task QueryUpForwarder_ForwardsQueryOutsideTheModule()
    {
        // Arrange
        var moduleResponse = new object();
        var moduleMock = new Mock<IModule>();
        moduleMock.Setup(module => module.Execute<OtherQuery, object>(
            It.IsAny<OtherQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(moduleResponse);
        var interceptorMock = new Mock<IQueryInterceptor<OtherQuery, object>>();
        interceptorMock.Setup(interceptor => interceptor.ExecuteAfter(It.IsAny<OtherQuery>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync((OtherQuery query, object result, CancellationToken token) => result);

        var expectedQuery = new OtherQuery(int.MinValue);
        var expectedToken = new CancellationToken();
        var handler = new QueryUpForwarder<TestQuery, string, OtherQuery, object>(moduleMock.Object, _ => expectedQuery, response => response.ToString()!, interceptorMock.Object);

        // Act
        var result = await handler.Handle(new TestQuery("query-name"), expectedToken);

        // Assert
        moduleMock.Verify(module => module.Execute<OtherQuery, object>(expectedQuery, expectedToken), Times.Once);
        interceptorMock.Verify(module => module.ExecuteBefore(expectedQuery, expectedToken), Times.Once);
        interceptorMock.Verify(module => module.ExecuteAfter(expectedQuery, moduleResponse, expectedToken), Times.Once);
        Assert.Equal(moduleResponse.ToString(), result);
    }
}
