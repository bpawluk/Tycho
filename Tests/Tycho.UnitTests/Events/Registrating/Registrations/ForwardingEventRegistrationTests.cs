using Moq;
using Tycho.Events;
using Tycho.Events.Model;
using Tycho.Events.Registrating.Registrations;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.Identity.Structure;
using Tycho.Modules.Instance;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Registrating.Registrations;

public class ForwardingEventRegistrationTests
{
    private readonly Mock<IModule<TestModule>> _moduleMock;
    private readonly Mock<IEventRouter> _eventRouterMock;

    public ForwardingEventRegistrationTests()
    {
        _eventRouterMock = new Mock<IEventRouter>();

        _moduleMock = new Mock<IModule<TestModule>>();
        _moduleMock.SetupGet(m => m.EventRouter)
                   .Returns(_eventRouterMock.Object);
    }

    [Fact]
    public async Task RouteAsync_WithBrokerReturningNoEvents_ReturnsEmpty()
    {
        // Arrange
        var publishId = Guid.NewGuid();
        var eventPayload = new TestEvent();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        _eventRouterMock.Setup(eb => eb.RouteAsync(publishId, eventPayload, cancellationToken))
                        .ReturnsAsync([]);

        var sut = new ForwardingEventRegistration<TestEvent, TestModule>(_moduleMock.Object);

        // Act
        IReadOnlyCollection<RoutedEvent> result = await sut.RouteAsync(publishId, eventPayload, cancellationToken);

        // Assert
        Assert.Empty(result);
        _eventRouterMock.Verify(eb => eb.RouteAsync(publishId, eventPayload, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task RouteAsync_WithBrokerReturningMultipleEvents_PreservesDestinationEndpoint()
    {
        // Arrange
        var publishId = Guid.NewGuid();
        var eventPayload = new TestEvent();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        RoutedEvent<TestEvent> firstRoutedEvent = CreateRoutedEvent(eventPayload);
        RoutedEvent<TestEvent> secondRoutedEvent = CreateRoutedEvent(eventPayload);

        _eventRouterMock.Setup(eb => eb.RouteAsync(publishId, eventPayload, cancellationToken))
                        .ReturnsAsync([firstRoutedEvent, secondRoutedEvent]);

        var sut = new ForwardingEventRegistration<TestEvent, TestModule>(_moduleMock.Object);

        // Act
        IReadOnlyCollection<RoutedEvent> result = await sut.RouteAsync(publishId, eventPayload, cancellationToken);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(firstRoutedEvent, result);
        Assert.Contains(secondRoutedEvent, result);
        Assert.Equal(InstanceIdentity.Parse("test-endpoint"), firstRoutedEvent.DestinationId);
        Assert.Equal(InstanceIdentity.Parse("test-endpoint"), secondRoutedEvent.DestinationId);
        _eventRouterMock.Verify(eb => eb.RouteAsync(publishId, eventPayload, cancellationToken), Times.Once);
    }

    private static RoutedEvent<TEvent> CreateRoutedEvent<TEvent>(TEvent payload)
        where TEvent : class, IEvent
    {
        var eventId = EventIdentity.Create<TEvent>();
        var handlerId = EventHandlerIdentity.Create<MultiEventHandler>();
        return new RoutedEvent<TEvent>(Guid.NewGuid(), Guid.NewGuid(), eventId, handlerId, InstanceIdentity.Parse("test-endpoint"), payload);
    }
}
