using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Events.Dispatching;
using Tycho.Events.Inbox;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
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
        _inboxConsumerMock = new Mock<IInboxConsumer>();
        var dispatcherMock = new Mock<IEventDispatcher>();


        var internals = new Internals(typeof(TestModule));
        internals.GetServiceCollection()
                 .AddTransient(_ => new InboxProcessorJob(_inboxConsumerMock.Object, dispatcherMock.Object));
        internals.Build();

        _sut = new InboxProcessorJobFactory(internals, _inboxConsumerMock.Object);
    }

    [Fact]
    public async Task CreateJobsAsync_WithReceivedEvents_ReturnsMatchingJobCount()
    {
        // Arrange
        var maxCount = 5;
        var cancellationToken = new CancellationToken();
        var entries = new List<RoutedEvent> { CreateRoutedEvent(), CreateRoutedEvent(), CreateRoutedEvent() };

        _inboxConsumerMock.Setup(i => i.Read(It.IsAny<int>(), cancellationToken))
                          .ReturnsAsync(entries);

        // Act
        var result = await _sut.CreateJobsAsync(maxCount, cancellationToken);

        // Assert
        Assert.Equal(entries.Count, result.Count);
        _inboxConsumerMock.Verify(i => i.Read(maxCount, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateJobsAsync_WithNoReceivedEvents_ReturnsEmptyCollection()
    {
        // Arrange
        var maxCount = 5;
        var cancellationToken = new CancellationToken();

        _inboxConsumerMock.Setup(i => i.Read(It.IsAny<int>(), cancellationToken))
                          .ReturnsAsync([]);

        // Act
        var result = await _sut.CreateJobsAsync(maxCount, cancellationToken);

        // Assert
        Assert.Empty(result);
        _inboxConsumerMock.Verify(i => i.Read(maxCount, cancellationToken), Times.Once);
    }

    private static RoutedEvent<TestEvent> CreateRoutedEvent()
    {
        var handlerId = EventHandlerIdentity.Create<TestEventHandler, TestEvent>();
        return new RoutedEvent<TestEvent>(Guid.NewGuid(), handlerId, new TestEvent());
    }
}
