using Moq;
using Tycho.Events.Dispatching;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Inbox;

public class InboxProcessorJobTests
{
    private readonly Mock<IInboxConsumer> _inboxConsumerMock;
    private readonly Mock<IEventDispatcher> _dispatcherMock;

    private readonly InboxProcessorJob _sut;

    public InboxProcessorJobTests()
    {
        _inboxConsumerMock = new Mock<IInboxConsumer>();
        _dispatcherMock = new Mock<IEventDispatcher>();
        _sut = new InboxProcessorJob(_inboxConsumerMock.Object, _dispatcherMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoEventAssigned_DoesNotCallDispatcher()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        // Act
        await _sut.ExecuteAsync(cancellationToken);

        // Assert
        _dispatcherMock.Verify(d => d.DispatchAsync(It.IsAny<RoutedEvent<TestEvent>>(), It.IsAny<CancellationToken>()), Times.Never);
        _inboxConsumerMock.Verify(i => i.MarkAsHandled(It.IsAny<Guid>(), cancellationToken), Times.Never);
        _inboxConsumerMock.Verify(i => i.MarkAsFailed(It.IsAny<Guid>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithAssignedEvent_DispatchesEvent()
    {
        // Arrange
        var routedEvent = CreateRoutedEvent();
        var cancellationToken = new CancellationToken();

        _dispatcherMock
            .Setup(d => d.DispatchAsync(routedEvent, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        _sut.ForEvent(routedEvent);
        await _sut.ExecuteAsync(cancellationToken);

        // Assert
        _dispatcherMock.Verify(d => d.DispatchAsync(routedEvent, cancellationToken), Times.Once);
        _inboxConsumerMock.Verify(i => i.MarkAsHandled(routedEvent.Id, cancellationToken), Times.Never);
        _inboxConsumerMock.Verify(i => i.MarkAsFailed(routedEvent.Id, cancellationToken), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithAssignedEvent_WhenDispatcherThrows_MarksEventAsFailed()
    {
        // Arrange
        var routedEvent = CreateRoutedEvent();
        var cancellationToken = new CancellationToken();

        _dispatcherMock
            .Setup(d => d.DispatchAsync(routedEvent, cancellationToken))
            .ThrowsAsync(new Exception("dispatch failure"));

        _inboxConsumerMock
            .Setup(i => i.MarkAsFailed(routedEvent.Id, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        _sut.ForEvent(routedEvent);
        await _sut.ExecuteAsync(cancellationToken);

        // Assert
        _inboxConsumerMock.Verify(i => i.MarkAsHandled(routedEvent.Id, cancellationToken), Times.Never);
        _inboxConsumerMock.Verify(i => i.MarkAsFailed(routedEvent.Id, cancellationToken), Times.Once);
    }

    private static RoutedEvent<TestEvent> CreateRoutedEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new RoutedEvent<TestEvent>(Guid.NewGuid(), eventId, handlerId, Route.Create(), new TestEvent());
    }
}
