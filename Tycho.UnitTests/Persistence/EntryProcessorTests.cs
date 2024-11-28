using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using Tycho.Events;
using Tycho.Events.Routing;
using Tycho.Persistence;
using Tycho.Persistence.Processing;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Persistence;

public class EntryProcessorTests
{
    private readonly Mock<IEventHandler<TestEvent>> _eventHandlerMock;
    private readonly Mock<IEventRouter> _eventRouterMock;
    private readonly Mock<IPayloadSerializer> _payloadSerializerMock;
    private readonly FakeLogger<EntryProcessor> _fakeLogger;

    private readonly EntryProcessor _sut;

    public EntryProcessorTests()
    {
        _eventHandlerMock = new Mock<IEventHandler<TestEvent>>();

        _eventRouterMock = new Mock<IEventRouter>();
        _eventRouterMock.Setup(x => x.FindHandler(It.Is<HandlerIdentity>(id => id.MatchesEvent(typeof(TestEvent)))))
                        .Returns(_eventHandlerMock.Object);

        _payloadSerializerMock = new Mock<IPayloadSerializer>();
        _payloadSerializerMock.Setup(x => x.Deserialize(It.IsAny<Type>(), It.IsAny<TestEvent>()))
                              .Returns((Type _, object payload) => (payload as TestEvent)!);

        _fakeLogger = new FakeLogger<EntryProcessor>();

        _sut = new EntryProcessor(
            _eventRouterMock.Object, 
            _payloadSerializerMock.Object,
            _fakeLogger);
    }

    [Fact]
    public async Task Process_WhenHandlerFound_InvokesHandlerAndReturnsTrue()
    {
        // Arrange
        var eventData = new TestEvent();
        var entry = new OutboxEntry(
            new HandlerIdentity(typeof(TestEvent), typeof(TestEventHandler), typeof(TestModule)),
            eventData);

        // Act
        var result = await _sut.Process(entry);

        // Assert
        _eventHandlerMock.Verify(x => x.Handle(eventData, It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(result);
    }

    [Fact]
    public async Task Process_WhenHandlerFails_ReturnsFalse()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Error message");
        _eventHandlerMock.Setup(x => x.Handle(It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(expectedException);

        var eventData = new TestEvent();
        var entry = new OutboxEntry(
            new HandlerIdentity(typeof(TestEvent), typeof(TestEventHandler), typeof(TestModule)),
            eventData);

        // Act
        var result = await _sut.Process(entry);

        // Assert
        _eventHandlerMock.Verify(x => x.Handle(eventData, It.IsAny<CancellationToken>()), Times.Once);
        Assert.False(result);

        var logs = _fakeLogger.Collector.GetSnapshot();
        Assert.Single(logs);

        var log = logs.Single();
        Assert.Equal(LogLevel.Error, log.Level);
        Assert.Equal($"Failed to process entry with ID {entry.Id}", log.Message);
        Assert.NotNull(log.Exception);
        Assert.Equal(expectedException.Message, log.Exception.Message);
    }

    [Fact]
    public async Task Process_WhenHandlerNotFound_ReturnsFalse()
    {
        // Arrange
        var entry = new OutboxEntry(
            new HandlerIdentity(typeof(OtherEvent), typeof(OtherEventHandler), typeof(TestModule)),
            new OtherEvent());

        // Act
        var result = await _sut.Process(entry);

        // Assert
        Assert.False(result);

        var logs = _fakeLogger.Collector.GetSnapshot();
        Assert.Single(logs);

        var log = logs.Single();
        Assert.Equal(LogLevel.Error, log.Level);
        Assert.Equal($"Failed to process entry with ID {entry.Id}", log.Message);
        Assert.NotNull(log.Exception);
        Assert.Equal($"Missing event handler with ID {entry.HandlerIdentity}", log.Exception.Message);
    }
}