using Moq;
using Tycho.Events;
using Tycho.Events.Dispatching;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Dispatching;

public class EventDispatcherTests
{
    private readonly Mock<IEventHandlerProvider> _handlerProviderMock;
    private readonly Mock<IEventHandler<TestEvent>> _eventHandlerMock;
    private readonly EventDispatcher _sut;

    public EventDispatcherTests()
    {
        _handlerProviderMock = new Mock<IEventHandlerProvider>();
        _eventHandlerMock = new Mock<IEventHandler<TestEvent>>();
        _sut = new EventDispatcher(_handlerProviderMock.Object);
    }

    [Fact]
    public async Task DispatchAsync_WithMatchingHandler_DispatchesTheEvent()
    {
        // Arrange
        var routedEvent = CreateRoutedEvent(new());
        var cancellationToken = new CancellationToken();

        _handlerProviderMock.Setup(hp => hp.GetHandler<TestEvent>(routedEvent.HandlerId))
                            .Returns(_eventHandlerMock.Object);

        // Act
        await _sut.DispatchAsync(routedEvent, cancellationToken);

        // Assert
        _eventHandlerMock.Verify(
            eh => eh.HandleAsync(
                It.Is<EventContext<TestEvent>>(context =>
                    context.Id == routedEvent.Id &&
                    context.Payload == routedEvent.Payload),
                cancellationToken));
    }

    [Fact]
    public async Task DispatchAsync_WithMissingHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        var routedEvent = CreateRoutedEvent();
        var cancellationToken = new CancellationToken();

        _handlerProviderMock.Setup(hp => hp.GetHandler<TestEvent>(routedEvent.HandlerId))
                            .Returns<IEventHandler<TestEvent>>(null!);

        // Act
        Task Act() => _sut.DispatchAsync(routedEvent, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    private static RoutedEvent<TestEvent> CreateRoutedEvent(TestEvent? payload = null)
    {
        var handlerId = EventHandlerIdentity.Create<TestEventHandler, TestEvent>();
        return new RoutedEvent<TestEvent>(Guid.NewGuid(), handlerId, payload ?? new TestEvent());
    }
}
