using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        var internals = new Internals(typeof(TestModule), Host.CreateEmptyApplicationBuilder(default));
        IServiceCollection serviceCollection = internals.GetHostBuilder().Services;

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
        _outboxConsumerMock.Verify(o => o.MarkAsDeliveredAsync(It.IsAny<Guid>(), cancellationToken), Times.Never);
        _outboxConsumerMock.Verify(o => o.MarkAsFailedAsync(It.IsAny<Guid>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithAssignedEvent_DeliversEvent()
    {
        // Arrange
        OutboxEvent outboxEvent = CreateOutboxEvent();
        var cancellationToken = new CancellationToken();

        _brokerMock
            .Setup(b => b.DeliverAsync(outboxEvent.RoutedEvent, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        _sut.ForEvent(outboxEvent);
        await _sut.ExecuteAsync(cancellationToken);

        // Assert
        _brokerMock.Verify(b => b.DeliverAsync(outboxEvent.RoutedEvent, cancellationToken), Times.Once);
        _outboxConsumerMock.Verify(o => o.MarkAsDeliveredAsync(outboxEvent.ClaimId, cancellationToken), Times.Once);
        _outboxConsumerMock.Verify(o => o.MarkAsFailedAsync(outboxEvent.ClaimId, cancellationToken), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithAssignedEvent_WhenBrokerThrows_MarksEventAsFailed()
    {
        // Arrange
        OutboxEvent outboxEvent = CreateOutboxEvent();
        var cancellationToken = new CancellationToken();

        _brokerMock
            .Setup(b => b.DeliverAsync(outboxEvent.RoutedEvent, cancellationToken))
            .ThrowsAsync(new Exception("delivery failure"));

        // Act
        _sut.ForEvent(outboxEvent);
        await _sut.ExecuteAsync(cancellationToken);

        // Assert
        _brokerMock.Verify(b => b.DeliverAsync(outboxEvent.RoutedEvent, cancellationToken), Times.Once);
        _outboxConsumerMock.Verify(o => o.MarkAsDeliveredAsync(outboxEvent.ClaimId, cancellationToken), Times.Never);
        _outboxConsumerMock.Verify(o => o.MarkAsFailedAsync(outboxEvent.ClaimId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithAssignedEvent_WhenMarkingAsDeliveredThrows_MarksEventAsFailed()
    {
        // Arrange
        OutboxEvent outboxEvent = CreateOutboxEvent();
        var cancellationToken = new CancellationToken();

        _brokerMock
            .Setup(b => b.DeliverAsync(outboxEvent.RoutedEvent, cancellationToken))
            .Returns(Task.CompletedTask);

        _outboxConsumerMock
            .Setup(o => o.MarkAsDeliveredAsync(outboxEvent.ClaimId, cancellationToken))
            .ThrowsAsync(new Exception("outbox failure"));

        // Act
        _sut.ForEvent(outboxEvent);
        await _sut.ExecuteAsync(cancellationToken);

        // Assert
        _brokerMock.Verify(b => b.DeliverAsync(outboxEvent.RoutedEvent, cancellationToken), Times.Once);
        _outboxConsumerMock.Verify(o => o.MarkAsDeliveredAsync(outboxEvent.ClaimId, cancellationToken), Times.Once);
        _outboxConsumerMock.Verify(o => o.MarkAsFailedAsync(outboxEvent.ClaimId, cancellationToken), Times.Once);
    }

    private static OutboxEvent CreateOutboxEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        var routedEvent = new SerializedRoutedEvent(Guid.NewGuid(), Guid.NewGuid(), eventId, handlerId, Route.Create(), "{}");
        return new OutboxEvent(Guid.NewGuid(), routedEvent);
    }
}
