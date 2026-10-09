using Moq;
using Tycho.Events.Model;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;
using Tycho.Identity.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Routing;

public class RoutedEventTests
{
    [Fact]
    public void Constructor_WithEventMetadataAndPayload_PreservesThem()
    {
        // Arrange
        var id = Guid.NewGuid();
        var publishId = Guid.NewGuid();
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        var payload = new TestEvent();

        // Act
        var sut = new RoutedEvent<TestEvent>(
            id, publishId, eventId, handlerId, InstanceIdentity.Parse("test-endpoint"), payload);

        // Assert
        Assert.Equal(id, sut.Id);
        Assert.Equal(publishId, sut.PublishId);
        Assert.Equal(eventId, sut.EventId);
        Assert.Equal(handlerId, sut.HandlerId);
        Assert.Same(payload, sut.Payload);
    }

    [Fact]
    public void Constructor_WithExplicitDestination_PreservesIt()
    {
        // Arrange
        InstanceIdentity destination = InstanceIdentity.Parse("test-endpoint");

        // Act
        var sut = new RoutedEvent<TestEvent>(
            Guid.NewGuid(), Guid.NewGuid(), EventIdentity.Create<TestEvent>(),
            EventHandlerIdentity.Create<TestEventHandler>(), destination, new TestEvent());

        // Assert
        Assert.Same(destination, sut.DestinationId);
    }

    [Fact]
    public void SerializePayloadWith_WithSerializer_ReturnsSerializedPayload()
    {
        // Arrange
        var payload = new TestEvent();
        string serializedPayload = "{}";
        var sut = new RoutedEvent<TestEvent>(
            Guid.NewGuid(),
            Guid.NewGuid(),
            EventIdentity.Create<TestEvent>(),
            EventHandlerIdentity.Create<TestEventHandler>(),
            InstanceIdentity.Parse("test-endpoint"),
            payload);

        var serializerMock = new Mock<IPayloadSerializer>();
        serializerMock.Setup(s => s.Serialize(payload))
                      .Returns(serializedPayload);

        // Act
        string result = sut.SerializePayloadWith(serializerMock.Object);

        // Assert
        Assert.Same(serializedPayload, result);
        serializerMock.Verify(s => s.Serialize(payload), Times.Once);
    }
}
