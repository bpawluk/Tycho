using Microsoft.Extensions.Hosting;
using Tycho.Events.Model;
using Tycho.Events.Registrating.Registrations;
using Tycho.Identity.Events;
using Tycho.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Registrating.Registrations;

public class FinalEventRegistrationTests
{
    [Fact]
    public void Constructor_WithRegularHandlerType_SetsDerivedHandlerId()
    {
        // Arrange
        using var internals = new AppInternals(Host.CreateEmptyApplicationBuilder(default), typeof(FinalEventRegistrationTests));

        // Act
        var sut = new FinalEventRegistration<TestEvent, TestEventHandler>(internals);

        // Assert
        Assert.Equal(EventHandlerIdentity.Create<TestEventHandler>(), sut.HandlerId);
    }

    [Fact]
    public async Task RouteAsync_WithAnyEvent_ReturnsSingleRoutedEventWithTheHandlerAndDestinationEndpoint()
    {
        // Arrange
        var publishId = Guid.NewGuid();
        var eventPayload = new TestEvent();
        using var internals = new AppInternals(Host.CreateEmptyApplicationBuilder(default), typeof(FinalEventRegistrationTests));
        var sut = new FinalEventRegistration<TestEvent, TestEventHandler>(internals);

        // Act
        IReadOnlyCollection<RoutedEvent> result = await sut.RouteAsync(
            publishId,
            eventPayload,
            TestContext.Current.CancellationToken);

        // Assert
        RoutedEvent<TestEvent> routedEvent = Assert.IsType<RoutedEvent<TestEvent>>(Assert.Single(result));

        Assert.Equal(publishId, routedEvent.PublishId);
        Assert.Same(eventPayload, routedEvent.Payload);
        Assert.Equal(sut.HandlerId, routedEvent.HandlerId);
        Assert.Equal(internals.OwnerId, routedEvent.DestinationId);
    }
}
