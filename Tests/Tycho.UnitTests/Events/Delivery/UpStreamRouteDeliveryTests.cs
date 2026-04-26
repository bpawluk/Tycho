using Moq;
using Tycho.Events.Broker;
using Tycho.Events.Delivery.Strategies;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Steps;
using Tycho.Identity.Events;
using Tycho.Structure.Parent;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Delivery;

public class UpStreamRouteDeliveryTests
{
    private readonly Mock<IParentReference> _parentReferenceMock;
    private readonly Mock<IEventBroker> _eventBrokerMock;
    private readonly UpStreamRouteDelivery _sut;

    public UpStreamRouteDeliveryTests()
    {
        _eventBrokerMock = new Mock<IEventBroker>();

        _parentReferenceMock = new Mock<IParentReference>();
        _parentReferenceMock.SetupGet(pr => pr.EventBroker)
                            .Returns(_eventBrokerMock.Object);

        _sut = new UpStreamRouteDelivery(_parentReferenceMock.Object);
    }

    [Fact]
    public void CanDeliver_WithUpStreamStepBeingNextInRoute_ReturnsTrue()
    {
        // Arrange
        var upStreamStep = UpStreamRouteStep.Create();
        var routedEvent = CreateRoutedEvent(upStreamStep);

        // Act
        var result = _sut.CanDeliver(routedEvent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanDeliver_WithDownStreamStepBeingNextInRoute_ReturnsFalse()
    {
        // Arrange
        var downStreamStep = DownStreamRouteStep.Create<TestModule>();
        var routedEvent = CreateRoutedEvent(downStreamStep);

        // Act
        var result = _sut.CanDeliver(routedEvent);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanDeliver_WithFinalStepBeingNextInRoute_ReturnsFalse()
    {
        // Arrange
        var finalStep = FinalRouteStep.Create();
        var routedEvent = CreateRoutedEvent(finalStep);

        // Act
        var result = _sut.CanDeliver(routedEvent);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanDeliver_WithEmptyRoute_ReturnsFalse()
    {
        // Arrange
        var routedEvent = CreateRoutedEvent();

        // Act
        var result = _sut.CanDeliver(routedEvent);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeliverAsync_WithUpStreamStepBeingNextInRoute_DeliversTheEvent()
    {
        // Arrange
        var upStreamStep = UpStreamRouteStep.Create();
        var routedEvent = CreateRoutedEvent(upStreamStep);
        var cancellationToken = new CancellationToken();

        // Act
        await _sut.DeliverAsync(routedEvent, cancellationToken);

        // Assert
        _eventBrokerMock.Verify(eb => eb.DeliverAsync(routedEvent, cancellationToken));
    }

    [Fact]
    public async Task DeliverAsync_WithDownStreamStepBeingNextInRoute_ThrowsInvalidOperationException()
    {
        // Arrange
        var downStreamStep = DownStreamRouteStep.Create<TestModule>();
        var routedEvent = CreateRoutedEvent(downStreamStep);
        var cancellationToken = new CancellationToken();

        // Act
        Task Act() => _sut.DeliverAsync(routedEvent, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    [Fact]
    public async Task DeliverAsync_WithFinalStepBeingNextInRoute_ThrowsInvalidOperationException()
    {
        // Arrange
        var finalStep = FinalRouteStep.Create();
        var routedEvent = CreateRoutedEvent(finalStep);
        var cancellationToken = new CancellationToken();

        // Act
        Task Act() => _sut.DeliverAsync(routedEvent, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    [Fact]
    public async Task DeliverAsync_WithEmptyRoute_ThrowsInvalidOperationException()
    {
        // Arrange
        var routedEvent = CreateRoutedEvent();
        var cancellationToken = new CancellationToken();

        // Act
        Task Act() => _sut.DeliverAsync(routedEvent, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    private static RoutedEvent<TestEvent> CreateRoutedEvent(IRouteStep? nextRouteStep = null)
    {
        var route = new Route();
        if (nextRouteStep != null)
        {
            route.Push(nextRouteStep);
        }
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new RoutedEvent<TestEvent>(Guid.NewGuid(), handlerId, route, new TestEvent());
    }
}
