using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using UnitTests.Utils;

namespace UnitTests.Messaging.Handlers;

public class TransientHandlerTests
{
    private int _testHandlerCreatorInvocationCount;

    public TransientHandlerTests() 
    {
        _testHandlerCreatorInvocationCount = 0;
    }

    [Fact]
    public async Task TransientEventHandler_CreatesNewHandlerEachTime()
    {
        // Arrange
        var handler = new TransientEventHandler<TestEvent>(TestHandlerCreator<TestMessageHandler>);

        // Act & Assert
        await handler.Handle(new TestEvent("test-event"), CancellationToken.None);
        Assert.Equal(1, _testHandlerCreatorInvocationCount);

        await handler.Handle(new TestEvent("test-event"), CancellationToken.None);
        Assert.Equal(2, _testHandlerCreatorInvocationCount);
    }

    [Fact]
    public async Task TransientRequestHandler_CreatesNewHandlerEachTime()
    {
        // Arrange
        var handler = new TransientRequestHandler<TestRequest>(TestHandlerCreator<TestMessageHandler>);

        // Act & Assert
        await handler.Handle(new TestRequest("test-request"), CancellationToken.None);
        Assert.Equal(1, _testHandlerCreatorInvocationCount);

        await handler.Handle(new TestRequest("test-request"), CancellationToken.None);
        Assert.Equal(2, _testHandlerCreatorInvocationCount);
    }

    [Fact]
    public async Task TransientRequestWithResponseHandler_CreatesNewHandlerEachTime()
    {
        // Arrange
        var handler = new TransientRequestHandler<TestRequestWithResponse, string>(TestHandlerCreator<TestMessageHandler>);

        // Act & Assert
        await handler.Handle(new TestRequestWithResponse("test-request"), CancellationToken.None);
        Assert.Equal(1, _testHandlerCreatorInvocationCount);

        await handler.Handle(new TestRequestWithResponse("test-request"), CancellationToken.None);
        Assert.Equal(2, _testHandlerCreatorInvocationCount);
    }

    private T TestHandlerCreator<T>()
    {
        _testHandlerCreatorInvocationCount++;
        return Activator.CreateInstance<T>();
    }
}
