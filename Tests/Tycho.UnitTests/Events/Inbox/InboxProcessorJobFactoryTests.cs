using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.Processor;
using Tycho.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Inbox;

public class InboxProcessorJobFactoryTests
{
    private readonly Mock<IInboxConsumer> _inboxConsumerMock;
    private readonly InboxProcessorJobFactory _sut;

    public InboxProcessorJobFactoryTests()
    {
        var internals = new Internals(typeof(TestModule));
        IServiceCollection serviceCollection = internals.GetServiceCollection();

        _inboxConsumerMock = new Mock<IInboxConsumer>();
        serviceCollection.AddSingleton(_inboxConsumerMock.Object);

        internals.Build();
        _sut = new InboxProcessorJobFactory(internals);
    }

    [Fact]
    public async Task CreateJobsAsync_WithReceivedEvents_ReturnsMatchingJobCount()
    {
        // Arrange
        int maxCount = 5;
        var cancellationToken = new CancellationToken();
        var entries = new List<RoutedEvent> { CreateRoutedEvent(), CreateRoutedEvent(), CreateRoutedEvent() };

        _inboxConsumerMock.Setup(i => i.Read(It.IsAny<int>(), cancellationToken))
                          .ReturnsAsync(entries);

        // Act
        IReadOnlyCollection<IJob> result = await _sut.CreateJobsAsync(maxCount, cancellationToken);

        // Assert
        Assert.Equal(entries.Count, result.Count);
        _inboxConsumerMock.Verify(i => i.Read(maxCount, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateJobsAsync_WithNoReceivedEvents_ReturnsEmptyCollection()
    {
        // Arrange
        int maxCount = 5;
        var cancellationToken = new CancellationToken();

        _inboxConsumerMock.Setup(i => i.Read(It.IsAny<int>(), cancellationToken))
                          .ReturnsAsync([]);

        // Act
        IReadOnlyCollection<IJob> result = await _sut.CreateJobsAsync(maxCount, cancellationToken);

        // Assert
        Assert.Empty(result);
        _inboxConsumerMock.Verify(i => i.Read(maxCount, cancellationToken), Times.Once);
    }

    private static RoutedEvent<TestEvent> CreateRoutedEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new RoutedEvent<TestEvent>(Guid.NewGuid(), eventId, handlerId, Route.Create(), new TestEvent());
    }
}
