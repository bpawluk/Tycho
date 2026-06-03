using Moq;
using Tycho.Events.Broker;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Events.Publishing;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Publishing;

public class EventPublisherTests
{
    private readonly Mock<IEventBroker> _eventBrokerMock;
    private readonly Mock<IOutboxWriter> _outboxWriterMock;

    private readonly IEventPublisher _sut;

    public EventPublisherTests()
    {
        _eventBrokerMock = new Mock<IEventBroker>();
        _outboxWriterMock = new Mock<IOutboxWriter>();
        _sut = new EventPublisher(_eventBrokerMock.Object, _outboxWriterMock.Object);
    }

    [Fact]
    public async Task PublishAsync_WithMissingPayload_ThrowsArgumentNullException()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        // Act
        Task Act() => _sut.PublishAsync<TestEvent>(null!, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Act);
    }

    [Fact]
    public async Task PublishAsync_WithMultipleRoutedEvents_WritesAllToOutbox()
    {
        // Arrange
        var eventPayload = new TestEvent();
        var cancellationToken = new CancellationToken();
        var routedEvents = new List<RoutedEvent>
        {
            CreateRoutedEvent(eventPayload),
            CreateRoutedEvent(eventPayload),
        };

        _eventBrokerMock.Setup(eb => eb.Route(It.IsAny<Guid>(), eventPayload))
                        .Returns(routedEvents);

        // Act
        await _sut.PublishAsync(eventPayload, cancellationToken);

        // Assert
        _outboxWriterMock.Verify(ow => ow.Write(routedEvents, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_WithNoRoutedEvents_DoesNotWriteToOutbox()
    {
        // Arrange
        var eventPayload = new TestEvent();
        var routedEvents = new List<RoutedEvent>();
        var cancellationToken = new CancellationToken();

        _eventBrokerMock.Setup(eb => eb.Route(It.IsAny<Guid>(), eventPayload))
                        .Returns(routedEvents);

        // Act
        await _sut.PublishAsync(eventPayload, cancellationToken);

        // Assert
        _outboxWriterMock.Verify(
            ow => ow.Write(It.IsAny<IReadOnlyCollection<RoutedEvent>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task PublishAsync_WithNullRoutedEvents_DoesNotWriteToOutbox()
    {
        // Arrange
        var eventPayload = new TestEvent();
        var cancellationToken = new CancellationToken();

        _eventBrokerMock.Setup(eb => eb.Route(It.IsAny<Guid>(), eventPayload))
                        .Returns<IReadOnlyCollection<RoutedEvent>>(null!);

        // Act
        await _sut.PublishAsync(eventPayload, cancellationToken);

        // Assert
        _outboxWriterMock.Verify(
            ow => ow.Write(It.IsAny<IReadOnlyCollection<RoutedEvent>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static RoutedEvent<TestEvent> CreateRoutedEvent(TestEvent? payload = null)
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new RoutedEvent<TestEvent>(Guid.NewGuid(), Guid.NewGuid(), eventId, handlerId, Route.Create(), payload ?? new TestEvent());
    }
}
