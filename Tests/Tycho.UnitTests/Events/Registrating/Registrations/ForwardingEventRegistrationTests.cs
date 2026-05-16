using Moq;
using Tycho.Events;
using Tycho.Events.Broker;
using Tycho.Events.Model;
using Tycho.Events.Registrating.Registrations;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Steps;
using Tycho.Identity.Events;
using Tycho.Identity.Modules;
using Tycho.Modules.Instance;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Registrating.Registrations;

public class ForwardingEventRegistrationTests
{
    private readonly Mock<IModule<TestModule>> _moduleMock;
    private readonly Mock<IEventBroker> _eventBrokerMock;

    public ForwardingEventRegistrationTests()
    {
        _eventBrokerMock = new Mock<IEventBroker>();

        _moduleMock = new Mock<IModule<TestModule>>();
        _moduleMock.SetupGet(m => m.EventBroker)
                   .Returns(_eventBrokerMock.Object);
    }

    [Fact]
    public void Route_WithBrokerReturningNoEvents_ReturnsEmpty()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var eventPayload = new TestEvent();

        _eventBrokerMock.Setup(eb => eb.Route(eventId, eventPayload))
                        .Returns([]);

        var sut = new ForwardingEventRegistration<TestEvent, TestModule>(_moduleMock.Object);

        // Act
        IReadOnlyCollection<RoutedEvent> result = sut.Route(eventId, eventPayload);

        // Assert
        Assert.Empty(result);
        _eventBrokerMock.Verify(eb => eb.Route(eventId, eventPayload), Times.Once);
    }

    [Fact]
    public void Route_WithBrokerReturningMultipleEvents_PushesDownStreamStepForTargetModuleOntoEachRoute()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var eventPayload = new TestEvent();
        RoutedEvent<TestEvent> firstRoutedEvent = CreateRoutedEvent(eventPayload);
        RoutedEvent<TestEvent> secondRoutedEvent = CreateRoutedEvent(eventPayload);

        _eventBrokerMock.Setup(eb => eb.Route(eventId, eventPayload))
                        .Returns([firstRoutedEvent, secondRoutedEvent]);

        var sut = new ForwardingEventRegistration<TestEvent, TestModule>(_moduleMock.Object);

        // Act
        IReadOnlyCollection<RoutedEvent> result = sut.Route(eventId, eventPayload);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(firstRoutedEvent, result);
        Assert.Contains(secondRoutedEvent, result);
        AssertRouteStartsWithDownStream(firstRoutedEvent.Route);
        AssertRouteStartsWithDownStream(secondRoutedEvent.Route);
        _eventBrokerMock.Verify(eb => eb.Route(eventId, eventPayload), Times.Once);
    }

    [Fact]
    public void Route_WithMappedRegistration_AndBrokerReturningNoEvents_ReturnsEmpty()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var eventPayload = new TestEvent();
        var mappedPayload = new OtherEvent();

        var mapMock = new Mock<Func<TestEvent, OtherEvent>>();
        mapMock.Setup(m => m(eventPayload))
               .Returns(mappedPayload);

        _eventBrokerMock.Setup(eb => eb.Route(eventId, mappedPayload))
                        .Returns([]);

        var sut = new MappedForwardingEventRegistration<TestEvent, OtherEvent, TestModule>(_moduleMock.Object, mapMock.Object);

        // Act
        IReadOnlyCollection<RoutedEvent> result = sut.Route(eventId, eventPayload);

        // Assert
        Assert.Empty(result);
        mapMock.Verify(m => m(eventPayload), Times.Once);
        _eventBrokerMock.Verify(eb => eb.Route(eventId, mappedPayload), Times.Once);
    }

    [Fact]
    public void Route_WithMappedRegistration_AndBrokerReturningMultipleEvents_PushesDownStreamStepForTargetModuleOntoEachRoute()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var eventPayload = new TestEvent();
        var mappedPayload = new OtherEvent();
        RoutedEvent<TestEvent> firstRoutedEvent = CreateRoutedEvent(eventPayload);
        RoutedEvent<TestEvent> secondRoutedEvent = CreateRoutedEvent(eventPayload);

        var mapMock = new Mock<Func<TestEvent, OtherEvent>>();
        mapMock.Setup(m => m(eventPayload))
               .Returns(mappedPayload);

        _eventBrokerMock.Setup(eb => eb.Route(eventId, mappedPayload))
                        .Returns([firstRoutedEvent, secondRoutedEvent]);

        var sut = new MappedForwardingEventRegistration<TestEvent, OtherEvent, TestModule>(_moduleMock.Object, mapMock.Object);

        // Act
        IReadOnlyCollection<RoutedEvent> result = sut.Route(eventId, eventPayload);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(firstRoutedEvent, result);
        Assert.Contains(secondRoutedEvent, result);
        AssertRouteStartsWithDownStream(firstRoutedEvent.Route);
        AssertRouteStartsWithDownStream(secondRoutedEvent.Route);
        mapMock.Verify(m => m(eventPayload), Times.Once);
        _eventBrokerMock.Verify(eb => eb.Route(eventId, mappedPayload), Times.Once);
    }

    private static RoutedEvent<TEvent> CreateRoutedEvent<TEvent>(TEvent payload)
        where TEvent : class, IEvent
    {
        var eventId = EventIdentity.Create<TEvent>();
        var handlerId = EventHandlerIdentity.Create<MultiEventHandler>();
        return new RoutedEvent<TEvent>(Guid.NewGuid(), eventId, handlerId, Route.Create(), payload);
    }

    private static void AssertRouteStartsWithDownStream(Route route)
    {
        Assert.Equal(2, route.Count);

        IRouteStep[] routeSteps = [.. route];
        DownStreamRouteStep downStreamRouteStep = Assert.IsType<DownStreamRouteStep>(routeSteps[0]);

        Assert.Equal(ModuleIdentity.Create<TestModule>(), downStreamRouteStep.Destination);
        Assert.IsType<FinalRouteStep>(routeSteps[1]);
    }
}
