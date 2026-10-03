using Moq;
using Tycho.Events;
using Tycho.Events.Model;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Steps;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Routing;

public class RoutedEventTests
{
    [Fact]
    public void Constructor_WithDefaultRoute_CreatesRouteWithFinalStep()
    {
        // Arrange
        var id = Guid.NewGuid();
        var route = Route.Create();

        // Act
        RoutedEvent<TestEvent> result = CreateRoutedEvent(id: id, route: route);

        // Assert
        Assert.NotNull(result.Route);
        IRouteStep step = Assert.Single(result.Route);
        Assert.IsType<FinalRouteStep>(step);
    }

    [Fact]
    public void Constructor_WithExplicitRoute_UsesProvidedRoute()
    {
        // Arrange
        var route = Route.Create();
        route.Push(UpStreamRouteStep.Create());

        // Act
        RoutedEvent<TestEvent> result = CreateRoutedEvent(route: route);

        // Assert
        Assert.Same(route, result.Route);
    }

    [Fact]
    public void SerializePayloadWith_WithSerializer_ReturnsSerializedPayload()
    {
        // Arrange
        var payload = new TestEvent();
        string serializedPayload = "{}";
        RoutedEvent<TestEvent> routedEvent = CreateRoutedEvent(payload: payload);

        var serializerMock = new Mock<IPayloadSerializer>();
        serializerMock.Setup(s => s.Serialize(payload))
                      .Returns(serializedPayload);

        // Act
        string result = routedEvent.SerializePayloadWith(serializerMock.Object);

        // Assert
        Assert.Same(serializedPayload, result);
        serializerMock.Verify(s => s.Serialize(payload), Times.Once);
    }

    [Fact]
    public void GetHandlerFrom_WithProvider_ReturnsHandlerResolvedByHandlerId()
    {
        // Arrange
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        RoutedEvent<TestEvent> routedEvent = CreateRoutedEvent(handlerId: handlerId);

        var handlerMock = new Mock<IEventHandler<TestEvent>>();

        var providerMock = new Mock<IEventHandlerProvider>();
        providerMock.Setup(p => p.GetHandler<TestEvent>(handlerId))
                    .Returns(handlerMock.Object);

        // Act
        IEventHandler result = routedEvent.GetHandlerFrom(providerMock.Object);

        // Assert
        Assert.Same(handlerMock.Object, result);
        providerMock.Verify(p => p.GetHandler<TestEvent>(handlerId), Times.Once);
    }

    [Fact]
    public async Task HandleWith_WithTypedHandler_InvokesHandleAsyncWithEventContext()
    {
        // Arrange
        var id = Guid.NewGuid();
        var payload = new TestEvent();
        var cancellationToken = new CancellationToken();
        RoutedEvent<TestEvent> routedEvent = CreateRoutedEvent(id: id, payload: payload);

        var handlerMock = new Mock<IEventHandler<TestEvent>>();
        handlerMock.Setup(h => h.HandleAsync(It.IsAny<EventContext<TestEvent>>(), cancellationToken))
                   .Returns(Task.CompletedTask);

        // Act
        await routedEvent.HandleWith(handlerMock.Object, cancellationToken);

        // Assert
        handlerMock.Verify(
            h => h.HandleAsync(
                It.Is<EventContext<TestEvent>>(c => c.Id == id && c.Payload == payload),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWith_WithDifferentHandlerType_ThrowsArgumentException()
    {
        // Arrange
        RoutedEvent<TestEvent> routedEvent = CreateRoutedEvent();
        var otherHandlerMock = new Mock<IEventHandler<OtherEvent>>();

        // Act
        Task Act() => routedEvent.HandleWith(otherHandlerMock.Object, new CancellationToken());

        // Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(Act);
        Assert.Contains("IEventHandler<TestEvent>", exception.Message);
    }

    private static RoutedEvent<TestEvent> CreateRoutedEvent(
        Guid? id = null,
        Guid? publishId = null,
        EventHandlerIdentity? handlerId = null,
        Route? route = null,
        TestEvent? payload = null)
    {
        var eventId = EventIdentity.Create<TestEvent>();
        return new RoutedEvent<TestEvent>(
            id ?? Guid.NewGuid(),
            publishId ?? Guid.NewGuid(),
            eventId,
            handlerId ?? EventHandlerIdentity.Create<TestEventHandler>(),
            route ?? Route.Create(),
            payload ?? new TestEvent());
    }

}
