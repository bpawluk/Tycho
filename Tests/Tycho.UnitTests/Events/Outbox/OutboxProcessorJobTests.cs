using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Events.Broker;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Outbox;

public class OutboxProcessorJobTests
{
    private readonly Mock<IOutboxConsumer> _outboxConsumerMock;
    private readonly Mock<IEventBroker> _brokerMock;

    private readonly OutboxProcessorJob _sut;

    public OutboxProcessorJobTests()
    {
        var internals = new Internals(typeof(TestModule));
        var serviceCollection = internals.GetServiceCollection();

        _outboxConsumerMock = new Mock<IOutboxConsumer>();
        serviceCollection.AddSingleton(_outboxConsumerMock.Object);

        _brokerMock = new Mock<IEventBroker>();
        serviceCollection.AddSingleton(_brokerMock.Object);

        internals.Build();
        _sut = new OutboxProcessorJob(internals);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoEventAssigned_ReturnsEarly()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        // Act
        await _sut.ExecuteAsync(cancellationToken);

        // Assert
        _brokerMock.Verify(b => b.DeliverAsync(It.IsAny<SerializedRoutedEvent>(), cancellationToken), Times.Never);
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

        // Act
        _sut.ForEvent(routedEvent);
        await _sut.ExecuteAsync(cancellationToken);

        // Assert
        _brokerMock.Verify(b => b.DeliverAsync(routedEvent, cancellationToken), Times.Once);
        _outboxConsumerMock.Verify(o => o.MarkAsDelivered(routedEvent.Id, cancellationToken), Times.Never);
        _outboxConsumerMock.Verify(o => o.MarkAsFailed(routedEvent.Id, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithAssignedEvent_WhenMarkingAsDeliveredThrows_MarksEventAsFailed()
    {
        // Arrange
        var routedEvent = CreateRoutedEvent();
        var cancellationToken = new CancellationToken();

        _brokerMock
            .Setup(b => b.DeliverAsync(routedEvent, cancellationToken))
            .Returns(Task.CompletedTask);

        _outboxConsumerMock
            .Setup(o => o.MarkAsDelivered(routedEvent.Id, cancellationToken))
            .ThrowsAsync(new Exception("outbox failure"));

        // Act
        _sut.ForEvent(routedEvent);
        await _sut.ExecuteAsync(cancellationToken);

        // Assert
        _brokerMock.Verify(b => b.DeliverAsync(routedEvent, cancellationToken), Times.Once);
        _outboxConsumerMock.Verify(o => o.MarkAsDelivered(routedEvent.Id, cancellationToken), Times.Once);
        _outboxConsumerMock.Verify(o => o.MarkAsFailed(routedEvent.Id, cancellationToken), Times.Once);
    }

    private static SerializedRoutedEvent CreateRoutedEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new SerializedRoutedEvent(Guid.NewGuid(), eventId, handlerId, Route.Create(), new TestEvent());
    }
}
