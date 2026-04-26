using Moq;
using Tycho.Events.Broker;
using Tycho.Events.Outbox;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Outbox;

public class OutboxProcessorJobTests
{
    private readonly Mock<IOutboxConsumer> _outboxConsumerMock;
    private readonly Mock<IEventBroker> _brokerMock;

    private readonly OutboxProcessorJob _sut;

    public OutboxProcessorJobTests()
    {
        _outboxConsumerMock = new Mock<IOutboxConsumer>();
        _brokerMock = new Mock<IEventBroker>();
        _sut = new OutboxProcessorJob(_outboxConsumerMock.Object, _brokerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoEventAssigned_DoesNotCallBroker()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        // Act
        await _sut.ExecuteAsync(cancellationToken);

        // Assert
        _brokerMock.Verify(b => b.DeliverAsync(It.IsAny<RoutedEvent<TestEvent>>(), It.IsAny<CancellationToken>()), Times.Never);
        _outboxConsumerMock.Verify(o => o.MarkAsDelivered(It.IsAny<Guid>(), cancellationToken), Times.Never);
        _outboxConsumerMock.Verify(o => o.MarkAsFailed(It.IsAny<Guid>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithAssignedEvent_DeliversEvent()
    {
        // Arrange
        var routedEvent = CreateRoutedEvent();
        var cancellationToken = new CancellationToken();

        _brokerMock
            .Setup(b => b.DeliverAsync(routedEvent, cancellationToken))
            .Returns(Task.CompletedTask);

        _outboxConsumerMock
            .Setup(o => o.MarkAsDelivered(routedEvent.Id, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        _sut.ForEvent(routedEvent);
        await _sut.ExecuteAsync(cancellationToken);

        // Assert
        _brokerMock.Verify(b => b.DeliverAsync(routedEvent, cancellationToken), Times.Once);
        _outboxConsumerMock.Verify(o => o.MarkAsDelivered(routedEvent.Id, cancellationToken), Times.Once);
        _outboxConsumerMock.Verify(o => o.MarkAsFailed(routedEvent.Id, cancellationToken), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithAssignedEvent_WhenBrokerThrows_MarksEventAsFailed()
    {
        // Arrange
        var routedEvent = CreateRoutedEvent();
        var cancellationToken = new CancellationToken();

        _brokerMock
            .Setup(b => b.DeliverAsync(routedEvent, cancellationToken))
            .ThrowsAsync(new Exception("delivery failure"));

        _outboxConsumerMock
            .Setup(o => o.MarkAsFailed(routedEvent.Id, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        _sut.ForEvent(routedEvent);
        await _sut.ExecuteAsync(cancellationToken);

        // Assert
        _outboxConsumerMock.Verify(o => o.MarkAsDelivered(routedEvent.Id, cancellationToken), Times.Never);
        _outboxConsumerMock.Verify(o => o.MarkAsFailed(routedEvent.Id, cancellationToken), Times.Once);
    }

    private static RoutedEvent<TestEvent> CreateRoutedEvent()
    {
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new RoutedEvent<TestEvent>(Guid.NewGuid(), handlerId, new TestEvent());
    }
}
