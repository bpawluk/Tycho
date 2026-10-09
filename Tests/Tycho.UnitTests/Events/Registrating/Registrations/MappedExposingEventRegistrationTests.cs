using Moq;
using Tycho.Events;
using Tycho.Events.Model;
using Tycho.Events.Registrating.Registrations;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.Identity.Structure;
using Tycho.Structure.Parent;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Registrating.Registrations;

public class MappedExposingEventRegistrationTests
{
    private readonly Mock<IParentReference> _parentReferenceMock;
    private readonly Mock<IEventRouter> _eventRouterMock;

    public MappedExposingEventRegistrationTests()
    {
        _eventRouterMock = new Mock<IEventRouter>();

        _parentReferenceMock = new Mock<IParentReference>();
        _parentReferenceMock.SetupGet(pr => pr.EventRouter)
                            .Returns(_eventRouterMock.Object);
    }

    [Fact]
    public async Task RouteAsync_WithBrokerReturningNoEvents_ReturnsEmpty()
    {
        // Arrange
        var publishId = Guid.NewGuid();
        var eventPayload = new TestEvent();
        var mappedPayload = new OtherEvent();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        var mapMock = new Mock<Func<TestEvent, OtherEvent>>();
        mapMock.Setup(m => m(eventPayload))
               .Returns(mappedPayload);

        _eventRouterMock.Setup(eb => eb.RouteAsync(publishId, mappedPayload, cancellationToken))
                        .ReturnsAsync([]);

        var sut = new MappedExposingEventRegistration<TestEvent, OtherEvent>(_parentReferenceMock.Object, mapMock.Object);

        // Act
        IReadOnlyCollection<RoutedEvent> result = await sut.RouteAsync(publishId, eventPayload, cancellationToken);

        // Assert
        Assert.Empty(result);
        mapMock.Verify(m => m(eventPayload), Times.Once);
        _eventRouterMock.Verify(eb => eb.RouteAsync(publishId, mappedPayload, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task RouteAsync_WithBrokerReturningMultipleEvents_PreservesDestinationEndpoint()
    {
        // Arrange
        var publishId = Guid.NewGuid();
        var eventPayload = new TestEvent();
        var mappedPayload = new OtherEvent();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        RoutedEvent<OtherEvent> firstRoutedEvent = CreateRoutedEvent(mappedPayload);
        RoutedEvent<OtherEvent> secondRoutedEvent = CreateRoutedEvent(mappedPayload);

        var mapMock = new Mock<Func<TestEvent, OtherEvent>>();
        mapMock.Setup(m => m(eventPayload))
               .Returns(mappedPayload);

        _eventRouterMock.Setup(eb => eb.RouteAsync(publishId, mappedPayload, cancellationToken))
                        .ReturnsAsync([firstRoutedEvent, secondRoutedEvent]);

        var sut = new MappedExposingEventRegistration<TestEvent, OtherEvent>(_parentReferenceMock.Object, mapMock.Object);

        // Act
        IReadOnlyCollection<RoutedEvent> result = await sut.RouteAsync(publishId, eventPayload, cancellationToken);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(firstRoutedEvent, result);
        Assert.Contains(secondRoutedEvent, result);
        Assert.Equal(InstanceIdentity.Parse("test-endpoint"), firstRoutedEvent.DestinationId);
        Assert.Equal(InstanceIdentity.Parse("test-endpoint"), secondRoutedEvent.DestinationId);
        mapMock.Verify(m => m(eventPayload), Times.Once);
        _eventRouterMock.Verify(eb => eb.RouteAsync(publishId, mappedPayload, cancellationToken), Times.Once);
    }

    private static RoutedEvent<TEvent> CreateRoutedEvent<TEvent>(TEvent payload)
        where TEvent : class, IEvent
    {
        var eventId = EventIdentity.Create<TEvent>();
        var handlerId = EventHandlerIdentity.Create<MultiEventHandler>();
        return new RoutedEvent<TEvent>(Guid.NewGuid(), Guid.NewGuid(), eventId, handlerId, InstanceIdentity.Parse("test-endpoint"), payload);
    }
}
