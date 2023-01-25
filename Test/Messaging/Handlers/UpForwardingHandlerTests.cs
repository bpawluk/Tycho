using System.Threading.Tasks;
using System.Threading;
using Test.Utils;
using Tycho.Messaging.Handlers;
using Tycho;

namespace Test.Messaging.Handlers;

public class UpForwardingHandlerTests
{
    [Fact]
    public async Task UpForwardingEventHandler_PassesEventOutsideTheModule()
    {
        // Arrange
        var moduleMock = new Mock<IModule>();
        var expectedEvent = new OtherEvent(int.MinValue);
        var expectedToken = new CancellationToken();
        var handler = new UpForwardingEventHandler<TestEvent, OtherEvent>(
            moduleMock.Object, _ => expectedEvent);

        // Act
        await handler.Handle(new TestEvent("event-name"), expectedToken);

        // Assert
        moduleMock.Verify(module => module.Publish(expectedEvent, expectedToken), Times.Once);
    }

    [Fact]
    public async Task UpForwardingCommandHandler_ForwardsCommandOutsideTheModule()
    {
        // Arrange
        var moduleMock = new Mock<IModule>();
        var expectedCommand = new OtherCommand(int.MinValue);
        var expectedToken = new CancellationToken();
        var handler = new UpForwardingCommandHandler<TestCommand, OtherCommand>(
            moduleMock.Object, _ => expectedCommand);

        // Act
        await handler.Handle(new TestCommand("command-name"), expectedToken);

        // Assert
        moduleMock.Verify(module => module.Execute(expectedCommand, expectedToken), Times.Once);
    }

    [Fact]
    public async Task UpForwardingQueryHandler_ForwardsQueryOutsideTheModule()
    {
        // Arrange
        var moduleResponse = new object();
        var moduleMock = new Mock<IModule>();
        moduleMock.Setup(module => module.Execute<OtherQuery, object>(
            It.IsAny<OtherQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(moduleResponse);

        var expectedQuery = new OtherQuery(int.MinValue);
        var expectedToken = new CancellationToken();
        var handler = new UpForwardingQueryHandler<TestQuery, string, OtherQuery, object>(
            moduleMock.Object, _ => expectedQuery, response => response.ToString()!);

        // Act
        var result = await handler.Handle(new TestQuery("query-name"), expectedToken);

        // Assert
        moduleMock.Verify(module => module.Execute<OtherQuery, object>(expectedQuery, expectedToken), Times.Once);
        Assert.Equal(moduleResponse.ToString(), result);
    }
}
