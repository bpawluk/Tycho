using Moq;
using Tycho.Events.Dispatching;
using Tycho.Events.Model;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Steps;
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
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        var payload = new TestEvent();

        // Act
        var result = new RoutedEvent<TestEvent>(id, eventId, handlerId, Route.Create(), payload);

        // Assert
        Assert.NotNull(result.Route);
        var step = Assert.Single(result.Route);
        Assert.IsType<FinalRouteStep>(step);
    }

    [Fact]
    public void Constructor_WithExplicitRoute_UsesProvidedRoute()
    {
        // Arrange
        var id = Guid.NewGuid();
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        var payload = new TestEvent();
        var route = Route.Create();
        route.Push(UpStreamRouteStep.Create());

        // Act
        var result = new RoutedEvent<TestEvent>(id, eventId, handlerId, route, payload);

        // Assert
        Assert.Same(route, result.Route);
    }

    [Fact]
    public async Task DispatchAsync_CallsDispatcherDispatchAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = new CancellationToken();
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        var payload = new TestEvent();

        var sut = new RoutedEvent<TestEvent>(id, eventId, handlerId, Route.Create(), payload);

        var dispatcherMock = new Mock<IEventDispatcher>();
        dispatcherMock.Setup(d => d.DispatchAsync(sut, cancellationToken)).Returns(Task.CompletedTask);

        // Act
        await sut.DispatchWithAsync(dispatcherMock.Object, cancellationToken);
        // Assert
        dispatcherMock.Verify(d => d.DispatchAsync(sut, cancellationToken), Times.Once);
    }
}
