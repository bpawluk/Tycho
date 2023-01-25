using System.Threading;
using System.Threading.Tasks;
using Test.Utils;
using Tycho;
using Tycho.Messaging.Handlers;

namespace Test.Messaging.Handlers;

public class DownForwardingHandlerTests
{
    [Fact]
    public async Task DownForwardingEventHandler_PassesEventToAnotherModule()
    {
        // Arrange
        var moduleMock = new Mock<ISubmodule<TestModule>>();
        var expectedEvent = new OtherEvent(int.MinValue);
        var expectedToken = new CancellationToken();
        var handler = new DownForwardingEventHandler<TestEvent, OtherEvent, TestModule>(
            moduleMock.Object, _ => expectedEvent);

        // Act
        await handler.Handle(new TestEvent("event-name"), expectedToken);

        // Assert
        moduleMock.Verify(module => module.Publish(expectedEvent, expectedToken), Times.Once);
    }

    [Fact]
    public async Task DownForwardingCommandHandler_ForwardsCommandToAnotherModule()
    {
        // Arrange
        var moduleMock = new Mock<ISubmodule<TestModule>>();
        var expectedCommand = new OtherCommand(int.MinValue);
        var expectedToken = new CancellationToken();
        var handler = new DownForwardingCommandHandler<TestCommand, OtherCommand, TestModule>(
            moduleMock.Object, _ => expectedCommand);

        // Act
        await handler.Handle(new TestCommand("command-name"), expectedToken);

        // Assert
        moduleMock.Verify(module => module.Execute(expectedCommand, expectedToken), Times.Once);
    }

    [Fact]
    public async Task DownForwardingQueryHandler_ForwardsQueryToAnotherModule()
    {
        // Arrange
        var moduleResponse = new object();
        var moduleMock = new Mock<ISubmodule<TestModule>>();
        moduleMock.Setup(module => module.Execute<OtherQuery, object>(
            It.IsAny<OtherQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(moduleResponse);

        var expectedQuery = new OtherQuery(int.MinValue);
        var expectedToken = new CancellationToken();
        var handler = new DownForwardingQueryHandler<TestQuery, string, OtherQuery, object, TestModule>(
            moduleMock.Object, _ => expectedQuery, response => response.ToString()!);

        // Act
        var result = await handler.Handle(new TestQuery("query-name"), expectedToken);

        // Assert
        moduleMock.Verify(module => module.Execute<OtherQuery, object>(expectedQuery, expectedToken), Times.Once);
        Assert.Equal(moduleResponse.ToString(), result);
    }
}
