using Moq;
using Tycho.Events.Broker;
using Tycho.Events.Delivery.Strategies;
using Tycho.Events.Model;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Steps;
using Tycho.Identity.Events;
using Tycho.Identity.Modules;
using Tycho.Modules.Instance;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Delivery;

public class DownStreamRouteDeliveryTests
{
    private readonly Mock<IModuleProvider> _moduleProviderMock;
    private readonly Mock<IModule> _moduleMock;
    private readonly Mock<IEventBroker> _eventBrokerMock;
    private readonly ModuleIdentity _testModuleIdentity;

    private readonly DownStreamRouteDelivery _sut;

    public DownStreamRouteDeliveryTests()
    {
        _testModuleIdentity = ModuleIdentity.Create<TestModule>();
        _eventBrokerMock = new Mock<IEventBroker>();

        _moduleMock = new Mock<IModule>();
        _moduleMock.SetupGet(m => m.EventBroker)
                   .Returns(_eventBrokerMock.Object);

        _moduleProviderMock = new Mock<IModuleProvider>();
        _moduleProviderMock.Setup(mp => mp.GetModule(_testModuleIdentity))
                           .Returns(_moduleMock.Object);

        _sut = new DownStreamRouteDelivery(_moduleProviderMock.Object);
    }

    [Fact]
    public void CanDeliver_WithDownStreamStepBeingNextInRoute_ReturnsTrue()
    {
        // Arrange 
        var downStreamStep = DownStreamRouteStep.Create<TestModule>();
        SerializedRoutedEvent routedEvent = CreateRoutedEvent(downStreamStep);

        // Act
        bool result = _sut.CanDeliver(routedEvent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanDeliver_WithUpStreamStepBeingNextInRoute_ReturnsFalse()
    {
        // Arrange
        var upStreamStep = UpStreamRouteStep.Create();
        SerializedRoutedEvent routedEvent = CreateRoutedEvent(upStreamStep);

        // Act
        bool result = _sut.CanDeliver(routedEvent);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanDeliver_WithFinalStepBeingNextInRoute_ReturnsFalse()
    {
        // Arrange
        var finalStep = FinalRouteStep.Create();
        SerializedRoutedEvent routedEvent = CreateRoutedEvent(finalStep);

        // Act
        bool result = _sut.CanDeliver(routedEvent);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanDeliver_WithEmptyRoute_ReturnsFalse()
    {
        // Arrange
        SerializedRoutedEvent routedEvent = CreateRoutedEvent();

        // Act 
        bool result = _sut.CanDeliver(routedEvent);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeliverAsync_WithDownStreamStepBeingNextInRoute_AndSubmoduleInRegistry_DeliversTheEvent()
    {
        // Arrange 
        var downStreamStep = DownStreamRouteStep.Create<TestModule>();
        SerializedRoutedEvent routedEvent = CreateRoutedEvent(downStreamStep);
        var cancellationToken = new CancellationToken();

        // Act
        await _sut.DeliverAsync(routedEvent, cancellationToken);

        // Assert
        _moduleProviderMock.Verify(mp => mp.GetModule(_testModuleIdentity));
    }

    [Fact]
    public async Task DeliverAsync_WithDownStreamStepBeingNextInRoute_AndMissingSubmodule_ThrowsInvalidOperationException()
    {
        // Arrange
        var downStreamStep = DownStreamRouteStep.Create<TestModule>();
        SerializedRoutedEvent routedEvent = CreateRoutedEvent(downStreamStep);
        var cancellationToken = new CancellationToken();

        _moduleProviderMock.Setup(mp => mp.GetModule(_testModuleIdentity))
                           .Returns<IModule>(null!);

        // Act
        Task Act() => _sut.DeliverAsync(routedEvent, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    [Fact]
    public async Task DeliverAsync_WithUpStreamStepBeingNextInRoute_ThrowsInvalidOperationException()
    {
        // Arrange
        var upStreamStep = UpStreamRouteStep.Create();
        SerializedRoutedEvent routedEvent = CreateRoutedEvent(upStreamStep);
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
        SerializedRoutedEvent routedEvent = CreateRoutedEvent(finalStep);
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
        SerializedRoutedEvent routedEvent = CreateRoutedEvent();
        var cancellationToken = new CancellationToken();

        // Act
        Task Act() => _sut.DeliverAsync(routedEvent, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    private static SerializedRoutedEvent CreateRoutedEvent(IRouteStep? nextRouteStep = null)
    {
        var route = Route.Create();
        if (nextRouteStep != null)
        {
            route.Push(nextRouteStep);
        }
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new SerializedRoutedEvent(Guid.NewGuid(), Guid.NewGuid(), eventId, handlerId, route, "{}");
    }
}
