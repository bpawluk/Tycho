using Moq;
using Tycho.Events;
using Tycho.Events.Model;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;
using Tycho.Identity.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Serialization;

public class EventSerializerBaseTests
{
    private readonly Mock<IPayloadSerializer> _payloadSerializerMock = new(MockBehavior.Strict);
    private readonly TestEventSerializer _sut;

    public EventSerializerBaseTests()
    {
        _sut = new TestEventSerializer(_payloadSerializerMock.Object);
    }

    [Fact]
    public void Serialize_WithRoutedEvent_PreservesEnvelopeAndSerializesPayload()
    {
        // Arrange
        var payload = new TestEvent();
        var routedEvent = new RoutedEvent<TestEvent>(
            Guid.NewGuid(),
            Guid.NewGuid(),
            EventIdentity.Create<TestEvent>(),
            EventHandlerIdentity.Create<TestEventHandler>(),
            InstanceIdentity.Parse("module:instance"),
            payload);
        string serializedPayload = "{\"value\":1}";
        _payloadSerializerMock.Setup(serializer => serializer.Serialize(payload)).Returns(serializedPayload);

        // Act
        SerializedRoutedEvent result = _sut.Serialize(routedEvent);

        // Assert
        Assert.Equal(routedEvent.Id, result.Id);
        Assert.Equal(routedEvent.PublishId, result.PublishId);
        Assert.Equal(routedEvent.EventId, result.EventId);
        Assert.Equal(routedEvent.HandlerId, result.HandlerId);
        Assert.Equal(routedEvent.DestinationId, result.DestinationId);
        Assert.Equal(serializedPayload, result.Payload);
        _payloadSerializerMock.Verify(serializer => serializer.Serialize(payload), Times.Once);
        _payloadSerializerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Deserialize_WithRegisteredEvent_PreservesEnvelopeAndDeserializesPayload(bool routed)
    {
        // Arrange
        _sut.Register<TestEvent>();
        SerializedEvent serializedEvent = CreateSerializedEvent(EventIdentity.Create<TestEvent>(), routed);
        var payload = new TestEvent();
        _payloadSerializerMock.Setup(serializer => serializer.Deserialize<TestEvent>(serializedEvent.Payload)).Returns(payload);

        // Act
        Event result = _sut.Deserialize(serializedEvent);

        // Assert
        Event<TestEvent> typedEvent = Assert.IsType<Event<TestEvent>>(result);
        Assert.Equal(serializedEvent.Id, typedEvent.Id);
        Assert.Equal(serializedEvent.PublishId, typedEvent.PublishId);
        Assert.Equal(serializedEvent.EventId, typedEvent.EventId);
        Assert.Equal(serializedEvent.HandlerId, typedEvent.HandlerId);
        Assert.Same(payload, typedEvent.Payload);
        _payloadSerializerMock.Verify(serializer => serializer.Deserialize<TestEvent>(serializedEvent.Payload), Times.Once);
        _payloadSerializerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void Deserialize_WithUnregisteredEvent_ThrowsBeforeDeserializingPayload()
    {
        // Arrange
        _sut.Register<TestEvent>();
        SerializedEvent serializedEvent = CreateSerializedEvent(EventIdentity.Create<OtherEvent>());

        // Act
        void Act() => _sut.Deserialize(serializedEvent);

        // Assert
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(Act);
        Assert.Contains(serializedEvent.EventId.Value, exception.Message);
        _payloadSerializerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void RegisterEvent_WithSameTypeTwice_PreservesRegistration()
    {
        // Arrange
        _sut.Register<TestEvent>();
        SerializedEvent serializedEvent = CreateSerializedEvent(EventIdentity.Create<TestEvent>());
        var payload = new TestEvent();
        _payloadSerializerMock.Setup(serializer => serializer.Deserialize<TestEvent>(serializedEvent.Payload)).Returns(payload);

        // Act
        _sut.Register<TestEvent>();

        // Assert
        Event<TestEvent> result = Assert.IsType<Event<TestEvent>>(_sut.Deserialize(serializedEvent));
        Assert.Same(payload, result.Payload);
        _payloadSerializerMock.Verify(serializer => serializer.Deserialize<TestEvent>(serializedEvent.Payload), Times.Once);
        _payloadSerializerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void RegisterEvent_WithSharedId_ThrowsAndPreservesOriginalRegistration()
    {
        // Arrange
        _sut.Register<FirstEvent>();
        SerializedEvent serializedEvent = CreateSerializedEvent(EventIdentity.Create<FirstEvent>());
        var payload = new FirstEvent();
        _payloadSerializerMock.Setup(serializer => serializer.Deserialize<FirstEvent>(serializedEvent.Payload)).Returns(payload);

        // Act
        void Act() => _sut.Register<SecondEvent>();

        // Assert
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(Act);
        Assert.Contains("shared-event", exception.Message);
        Assert.Contains(nameof(FirstEvent), exception.Message);
        Assert.Contains(nameof(SecondEvent), exception.Message);
        Event<FirstEvent> result = Assert.IsType<Event<FirstEvent>>(_sut.Deserialize(serializedEvent));
        Assert.Same(payload, result.Payload);
        _payloadSerializerMock.Verify(serializer => serializer.Deserialize<FirstEvent>(serializedEvent.Payload), Times.Once);
        _payloadSerializerMock.VerifyNoOtherCalls();
    }

    private static SerializedEvent CreateSerializedEvent(EventIdentity eventId, bool routed = false)
    {
        var id = Guid.NewGuid();
        var publishId = Guid.NewGuid();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        string payload = "{\"value\":1}";

        return routed
            ? new SerializedRoutedEvent(id, publishId, eventId, handlerId, InstanceIdentity.Parse("module:instance"), payload)
            : new SerializedEvent(id, publishId, eventId, handlerId, payload);
    }

    private sealed class TestEventSerializer(IPayloadSerializer payloadSerializer) : EventSerializerBase(payloadSerializer)
    {
        public void Register<TEvent>() where TEvent : class, IEvent => RegisterEvent<TEvent>();
    }

    [TychoId("shared-event")]
    private sealed class FirstEvent : IEvent { }

    [TychoId("shared-event")]
    private sealed class SecondEvent : IEvent { }
}
