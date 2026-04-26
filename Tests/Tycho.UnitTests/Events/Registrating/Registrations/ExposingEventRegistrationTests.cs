using Moq;
using Tycho.Events;
using Tycho.Events.Broker;
using Tycho.Events.Model;
using Tycho.Events.Registrating.Registrations;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Steps;
using Tycho.Identity.Events;
using Tycho.Structure.Parent;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Registrating.Registrations;

public class ExposingEventRegistrationTests
{
	private readonly Mock<IParentReference> _parentReferenceMock;
	private readonly Mock<IEventBroker> _eventBrokerMock;

	public ExposingEventRegistrationTests()
	{
		_eventBrokerMock = new Mock<IEventBroker>();

		_parentReferenceMock = new Mock<IParentReference>();
		_parentReferenceMock.SetupGet(pr => pr.EventBroker)
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

		var sut = new ExposingEventRegistration<TestEvent>(_parentReferenceMock.Object);

		// Act
		var result = sut.Route(eventId, eventPayload);

		// Assert
		Assert.Empty(result);
		_eventBrokerMock.Verify(eb => eb.Route(eventId, eventPayload), Times.Once);
	}

	[Fact]
	public void Route_WithBrokerReturningMultipleEvents_PushesUpStreamStepOntoEachRoute()
	{
		// Arrange
		var eventId = Guid.NewGuid();
		var eventPayload = new TestEvent();
		var firstRoutedEvent = CreateRoutedEvent(eventPayload);
		var secondRoutedEvent = CreateRoutedEvent(eventPayload);

		_eventBrokerMock.Setup(eb => eb.Route(eventId, eventPayload))
						.Returns([firstRoutedEvent, secondRoutedEvent]);

		var sut = new ExposingEventRegistration<TestEvent>(_parentReferenceMock.Object);

		// Act
		var result = sut.Route(eventId, eventPayload);

		// Assert
		Assert.Equal(2, result.Count);
		Assert.Contains(firstRoutedEvent, result);
		Assert.Contains(secondRoutedEvent, result);
		AssertRouteStartsWithUpStream(firstRoutedEvent.Route);
		AssertRouteStartsWithUpStream(secondRoutedEvent.Route);
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

		var sut = new MappedExposingEventRegistration<TestEvent, OtherEvent>(_parentReferenceMock.Object, mapMock.Object);

		// Act
		var result = sut.Route(eventId, eventPayload);

		// Assert
		Assert.Empty(result);
		mapMock.Verify(m => m(eventPayload), Times.Once);
		_eventBrokerMock.Verify(eb => eb.Route(eventId, mappedPayload), Times.Once);
	}

	[Fact]
	public void Route_WithMappedRegistration_AndBrokerReturningMultipleEvents_PushesUpStreamStepOntoEachRoute()
	{
		// Arrange
		var eventId = Guid.NewGuid();
		var eventPayload = new TestEvent();
		var mappedPayload = new OtherEvent();
        var firstRoutedEvent = CreateRoutedEvent(eventPayload);
        var secondRoutedEvent = CreateRoutedEvent(eventPayload);

		var mapMock = new Mock<Func<TestEvent, OtherEvent>>();
		mapMock.Setup(m => m(eventPayload))
			   .Returns(mappedPayload);

		_eventBrokerMock.Setup(eb => eb.Route(eventId, mappedPayload))
						.Returns([firstRoutedEvent, secondRoutedEvent]);

		var sut = new MappedExposingEventRegistration<TestEvent, OtherEvent>(_parentReferenceMock.Object, mapMock.Object);

		// Act
		var result = sut.Route(eventId, eventPayload);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(firstRoutedEvent, result);
        Assert.Contains(secondRoutedEvent, result);
        AssertRouteStartsWithUpStream(firstRoutedEvent.Route);
        AssertRouteStartsWithUpStream(secondRoutedEvent.Route);
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

	private static void AssertRouteStartsWithUpStream(Route route)
	{
		Assert.Equal(2, route.Count);

		var routeSteps = route.ToArray();

		Assert.IsType<UpStreamRouteStep>(routeSteps[0]);
		Assert.IsType<FinalRouteStep>(routeSteps[1]);
	}
}
