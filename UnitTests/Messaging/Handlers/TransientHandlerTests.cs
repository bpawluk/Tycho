using System;
using System.Threading;
using System.Threading.Tasks;
using UnitTests.Utils;
using Tycho.Messaging.Handlers;

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
    public async Task TransientCommandHandler_CreatesNewHandlerEachTime()
    {
        // Arrange
        var handler = new TransientCommandHandler<TestCommand>(TestHandlerCreator<TestMessageHandler>);

        // Act & Assert
        await handler.Handle(new TestCommand("test-command"), CancellationToken.None);
        Assert.Equal(1, _testHandlerCreatorInvocationCount);

        await handler.Handle(new TestCommand("test-command"), CancellationToken.None);
        Assert.Equal(2, _testHandlerCreatorInvocationCount);
    }

    [Fact]
    public async Task TransientQueryHandler_CreatesNewHandlerEachTime()
    {
        // Arrange
        var handler = new TransientQueryHandler<TestQuery, string>(TestHandlerCreator<TestMessageHandler>);

        // Act & Assert
        await handler.Handle(new TestQuery("test-query"), CancellationToken.None);
        Assert.Equal(1, _testHandlerCreatorInvocationCount);

        await handler.Handle(new TestQuery("test-query"), CancellationToken.None);
        Assert.Equal(2, _testHandlerCreatorInvocationCount);
    }

    private T TestHandlerCreator<T>()
    {
        _testHandlerCreatorInvocationCount++;
        return Activator.CreateInstance<T>();
    }
}
