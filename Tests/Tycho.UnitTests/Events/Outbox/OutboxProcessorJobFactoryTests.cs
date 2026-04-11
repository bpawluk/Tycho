using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Events.Broker;
using Tycho.Events.Outbox;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
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
        _outboxConsumerMock = new Mock<IOutboxConsumer>();
        var brokerMock = new Mock<IEventBroker>();

        var internals = new Internals(typeof(TestModule));
        internals.GetServiceCollection()
                 .AddTransient(_ => new OutboxProcessorJob(_outboxConsumerMock.Object, brokerMock.Object));
        internals.Build();

        _sut = new OutboxProcessorJobFactory(internals, _outboxConsumerMock.Object);
    }

    [Fact]
    public async Task CreateJobsAsync_WithReceivedEvents_ReturnsMatchingJobCount()
    {
        // Arrange
        var maxCount = 5;
        var cancellationToken = new CancellationToken();
        var entries = new List<RoutedEvent> { CreateRoutedEvent(), CreateRoutedEvent(), CreateRoutedEvent() };

        _outboxConsumerMock.Setup(o => o.Read(It.IsAny<int>(), cancellationToken))
                           .ReturnsAsync(entries);

        // Act
        var result = await _sut.CreateJobsAsync(maxCount, cancellationToken);

        // Assert
        Assert.Equal(entries.Count, result.Count);
        _outboxConsumerMock.Verify(o => o.Read(maxCount, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateJobsAsync_WithNoReceivedEvents_ReturnsEmptyCollection()
    {
        // Arrange
        var maxCount = 5;
        var cancellationToken = new CancellationToken();

        _outboxConsumerMock.Setup(o => o.Read(It.IsAny<int>(), cancellationToken))
                           .ReturnsAsync([]);

        // Act
        var result = await _sut.CreateJobsAsync(maxCount, cancellationToken);

        // Assert
        Assert.Empty(result);
        _outboxConsumerMock.Verify(o => o.Read(maxCount, cancellationToken), Times.Once);
    }

    private static RoutedEvent<TestEvent> CreateRoutedEvent()
    {
        var handlerId = EventHandlerIdentity.Create<TestEventHandler, TestEvent>();
        return new RoutedEvent<TestEvent>(Guid.NewGuid(), handlerId, new TestEvent());
    }
}
