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
    public async Task CreateJobsAsync_WithReceivedEvents_ReturnsMatchingJobCount()
    {
        // Arrange
        int maxCount = 5;
        var cancellationToken = new CancellationToken();
        var entries = new List<SerializedRoutedEvent> { CreateRoutedEvent(), CreateRoutedEvent(), CreateRoutedEvent() };

        _outboxConsumerMock.Setup(o => o.Read(It.IsAny<int>(), cancellationToken))
                           .ReturnsAsync(entries);

        // Act
        IReadOnlyCollection<IJob> result = await _sut.CreateJobsAsync(maxCount, cancellationToken);

        // Assert
        Assert.Equal(entries.Count, result.Count);
        _outboxConsumerMock.Verify(o => o.Read(maxCount, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateJobsAsync_WithNoReceivedEvents_ReturnsEmptyCollection()
    {
        // Arrange
        int maxCount = 5;
        var cancellationToken = new CancellationToken();

        _outboxConsumerMock.Setup(o => o.Read(It.IsAny<int>(), cancellationToken))
                           .ReturnsAsync([]);

        // Act
        IReadOnlyCollection<IJob> result = await _sut.CreateJobsAsync(maxCount, cancellationToken);

        // Assert
        Assert.Empty(result);
        _outboxConsumerMock.Verify(o => o.Read(maxCount, cancellationToken), Times.Once);
    }

    private static SerializedRoutedEvent CreateRoutedEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new SerializedRoutedEvent(Guid.NewGuid(), eventId, handlerId, Route.Create(), "{}");
    }
}
