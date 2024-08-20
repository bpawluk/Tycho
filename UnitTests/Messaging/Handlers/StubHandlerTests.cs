using System.Threading;
using Tycho.Messaging.Handlers;
using UnitTests.Utils;

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
    public void StubRequestHandler_ReturnsCompletedTask()
    {
        // Arrange
        var handler = new StubRequestHandler<TestRequest>();

        // Act
        var result = handler.Handle(new TestRequest("test-request"), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsCompleted);
    }

    [Fact]
    public void StubRequestWithResponseHandler_ReturnsCompletedTaskWithResult()
    {
        // Arrange
        var expectedResult = "result";
        var handler = new StubRequestHandler<TestRequestWithResponse, string>(expectedResult);

        // Act
        var result = handler.Handle(new TestRequestWithResponse("test-request"), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsCompleted);
        Assert.Equal(expectedResult, result.Result);
    }
}
