using Tycho.Events.Model;
using Tycho.Events.Registrating.Registrations;
using Tycho.Events.Routing.Steps;
using Tycho.Identity.Events;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Registrating.Registrations;

public class FinalEventRegistrationTests
{
    [Fact]
    public void Constructor_WithRegularHandler_SetsHandlerAndDerivedHandlerId()
    {
        // Arrange
        var handler = new TestEventHandler();

        // Act
        var sut = new FinalEventRegistration<TestEvent, TestEventHandler>(handler);

        // Assert
        Assert.Same(handler, sut.Handler);
        Assert.Equal(EventHandlerIdentity.Create<TestEventHandler>(), sut.HandlerId);
    }

    [Fact]
    public void Route_WithAnyEvent_ReturnsSingleRoutedEventWithTheHandlerAndFinalRoute()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var eventPayload = new TestEvent();
        var handler = new TestEventHandler();
        var sut = new FinalEventRegistration<TestEvent, TestEventHandler>(handler);

        // Act
        var result = sut.Route(eventId, eventPayload);

        // Assert
        var routedEvent = Assert.IsType<RoutedEvent<TestEvent>>(Assert.Single(result));

        Assert.Equal(eventId, routedEvent.Id);
        Assert.Same(eventPayload, routedEvent.Payload);
        Assert.Equal(sut.HandlerId, routedEvent.HandlerId);
        Assert.Single(routedEvent.Route);
        Assert.IsType<FinalRouteStep>(routedEvent.Route.Peek());
    }

}
