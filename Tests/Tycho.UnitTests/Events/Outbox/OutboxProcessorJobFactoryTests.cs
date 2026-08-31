using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.Processor;
using Tycho.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Outbox;

public class OutboxProcessorJobFactoryTests
{
    private readonly Mock<IOutboxConsumer> _outboxConsumerMock;
    private readonly OutboxProcessorJobFactory _sut;

    public OutboxProcessorJobFactoryTests()
    {
        var internals = new Internals(typeof(TestModule));
        IServiceCollection serviceCollection = internals.GetServiceCollection();

        _outboxConsumerMock = new Mock<IOutboxConsumer>();
        serviceCollection.AddSingleton(_outboxConsumerMock.Object);

        internals.Build();
        _sut = new OutboxProcessorJobFactory(internals);
    }

    [Fact]
    public async Task TryCreateJobAsync_WithReceivedEvent_ReturnsJob()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        OutboxEvent outboxEvent = CreateOutboxEvent();

        _outboxConsumerMock.Setup(o => o.TryReadAsync(cancellationToken))
                           .ReturnsAsync(outboxEvent);

        // Act
        IJob? result = await _sut.TryCreateJobAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        _outboxConsumerMock.Verify(o => o.TryReadAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task TryCreateJobAsync_WithNoReceivedEvent_ReturnsNull()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        _outboxConsumerMock.Setup(o => o.TryReadAsync(cancellationToken))
                           .ReturnsAsync((OutboxEvent?)null);

        // Act
        IJob? result = await _sut.TryCreateJobAsync(cancellationToken);

        // Assert
        Assert.Null(result);
        _outboxConsumerMock.Verify(o => o.TryReadAsync(cancellationToken), Times.Once);
    }

    private static OutboxEvent CreateOutboxEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        var routedEvent = new SerializedRoutedEvent(Guid.NewGuid(), Guid.NewGuid(), eventId, handlerId, Route.Create(), "{}");
        return new OutboxEvent(Guid.NewGuid(), routedEvent);
    }
}
