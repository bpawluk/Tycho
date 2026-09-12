using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        var internals = new Internals(typeof(TestModule), Host.CreateEmptyApplicationBuilder(default));
        IServiceCollection serviceCollection = internals.GetHostBuilder().Services;

        _inboxConsumerMock = new Mock<IInboxConsumer>();
        serviceCollection.AddSingleton(_inboxConsumerMock.Object);

        internals.Build();
        _sut = new InboxProcessorJobFactory(internals);
    }

    [Fact]
    public async Task TryCreateJobAsync_WithReceivedEvent_ReturnsJob()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        InboxEvent inboxEvent = CreateInboxEvent();

        _inboxConsumerMock.Setup(i => i.TryReadAsync(cancellationToken))
                          .ReturnsAsync(inboxEvent);

        // Act
        IJob? result = await _sut.TryCreateJobAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        _inboxConsumerMock.Verify(i => i.TryReadAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task TryCreateJobAsync_WithNoReceivedEvent_ReturnsNull()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        _inboxConsumerMock.Setup(i => i.TryReadAsync(cancellationToken))
                          .ReturnsAsync((InboxEvent?)null);

        // Act
        IJob? result = await _sut.TryCreateJobAsync(cancellationToken);

        // Assert
        Assert.Null(result);
        _inboxConsumerMock.Verify(i => i.TryReadAsync(cancellationToken), Times.Once);
    }

    private static InboxEvent CreateInboxEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        var routedEvent = new RoutedEvent<TestEvent>(Guid.NewGuid(), Guid.NewGuid(), eventId, handlerId, Route.Create(), new TestEvent());
        return new InboxEvent(Guid.NewGuid(), routedEvent);
    }
}
