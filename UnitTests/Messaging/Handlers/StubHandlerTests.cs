using System.Threading;
using UnitTests.Utils;
using Tycho.Messaging.Handlers;

namespace UnitTests.Messaging.Handlers;

public class StubHandlerTests
{
    [Fact]
    public void StubEventHandler_ReturnsCompletedTask()
    {
        // Arrange
        var handler = new StubEventHandler<TestEvent>();

        // Act
        var result = handler.Handle(new TestEvent("test-event"), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsCompleted);
    }

    [Fact]
    public void StubCommandHandler_ReturnsCompletedTask()
    {
        // Arrange
        var handler = new StubCommandHandler<TestCommand>();

        // Act
        var result = handler.Handle(new TestCommand("test-command"), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsCompleted);
    }

    [Fact]
    public void StubQueryHandler_ReturnsCompletedTaskWithResult()
    {
        // Arrange
        var expectedResult = "result";
        var handler = new StubQueryHandler<TestQuery, string>(expectedResult);

        // Act
        var result = handler.Handle(new TestQuery("test-query"), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsCompleted);
        Assert.Equal(expectedResult, result.Result);
    }
}
