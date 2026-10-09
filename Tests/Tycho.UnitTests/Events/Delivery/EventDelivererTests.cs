using Moq;
using Tycho.Events.Delivery;
using Tycho.Events.Model;
using Tycho.Identity.Events;
using Tycho.Identity.Structure;
using Tycho.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Delivery;

public sealed class EventDelivererTests
{
    [Fact]
    public async Task DeliverAsync_WithRegisteredDestination_AcceptsOnlyAtThatEndpoint()
    {
        // Arrange
        SerializedRoutedEvent routedEvent = CreateSerializedRoutedEvent();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var destinationMock = new Mock<IDeliveryEndpoint>(MockBehavior.Strict);
        destinationMock.Setup(endpoint => endpoint.AcceptAsync(routedEvent, cancellationToken)).Returns(Task.CompletedTask);
        var otherEndpointMock = new Mock<IDeliveryEndpoint>(MockBehavior.Strict);
        var controlPlane = new ControlPlane(InstanceIdentity.Create(typeof(EventDelivererTests)));
        controlPlane.RegisterModule(routedEvent.DestinationId, new ModuleReference(destinationMock.Object));
        controlPlane.RegisterModule(InstanceIdentity.Parse("other-module"), new ModuleReference(otherEndpointMock.Object));
        controlPlane.CompleteRegistration();
        var sut = new EventDeliverer(controlPlane);

        // Act
        await sut.DeliverAsync(routedEvent, cancellationToken);

        // Assert
        destinationMock.Verify(endpoint => endpoint.AcceptAsync(routedEvent, cancellationToken), Times.Once);
        destinationMock.VerifyNoOtherCalls();
        otherEndpointMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeliverAsync_WithUnregisteredDestination_ThrowsWithoutCallingOtherEndpoints()
    {
        // Arrange
        SerializedRoutedEvent routedEvent = CreateSerializedRoutedEvent();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var otherEndpointMock = new Mock<IDeliveryEndpoint>(MockBehavior.Strict);
        var controlPlane = new ControlPlane(InstanceIdentity.Create(typeof(EventDelivererTests)));
        controlPlane.RegisterModule(InstanceIdentity.Parse("other-module"), new ModuleReference(otherEndpointMock.Object));
        controlPlane.CompleteRegistration();
        var sut = new EventDeliverer(controlPlane);

        // Act
        Task Act() => sut.DeliverAsync(routedEvent, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);

        otherEndpointMock.VerifyNoOtherCalls();
    }

    private static SerializedRoutedEvent CreateSerializedRoutedEvent()
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        return new SerializedRoutedEvent(Guid.NewGuid(), Guid.NewGuid(), eventId, handlerId, InstanceIdentity.Parse("test-endpoint"), "{}");
    }

}
