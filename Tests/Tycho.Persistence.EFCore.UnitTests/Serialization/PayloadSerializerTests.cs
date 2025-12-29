using System.Text.Json;
using Tycho.Events;
using Tycho.Persistence.EFCore.Serialization;
using Tycho.Persistence.EFCore.UnitTests._Data.Events;

namespace Tycho.Persistence.EFCore.UnitTests.Serialization;

public class PayloadSerializerTests
{
    private readonly PayloadSerializer _sut = new();

    public static IEnumerable<object?[]> NonStringPayloads =>
    [
        [null],
        [123],
        [new object()]
    ];

    [Fact]
    public void Serialize_WithValidEventData_ReturnsSerializedPayload()
    {
        // Arrange
        var eventData = new TestEventWithData();

        // Act
        var payload = _sut.Serialize(eventData);

        // Assert
        Assert.Equal(eventData.GetSerializedPayload(), payload);
    }

    [Fact]
    public void Serialize_WithNullEventData_ThrowsArgumentNullException()
    {
        // Arrange
        IEvent eventData = null!;

        // Act
        void Act()
        {
            _sut.Serialize(eventData);
        }

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Fact]
    public void Deserialize_WithValidPayload_ReturnsDeserializedEventData()
    {
        // Arrange
        var expectedEventData = new TestEventWithData();
        var payload = expectedEventData.GetSerializedPayload();

        // Act
        var result = _sut.Deserialize(typeof(TestEventWithData), payload);

        // Assert
        Assert.True(expectedEventData.EqualsEvent(result as TestEventWithData));
    }


    [Theory]
    [MemberData(nameof(NonStringPayloads))]
    public void Deserialize_WithNonStringPayload_ThrowsArgumentException(object? payload)
    {
        // Arrange
        // - no arrangement required

        // Act
        void Act()
        {
            _sut.Deserialize(typeof(TestEvent), payload!);
        }

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Deserialize_WithInvalidPayloadFormat_ThrowsInvalidOperationException()
    {
        // Arrange
        var payload = "{property='invalidFormat'}";

        // Act
        void Act()
        {
            _sut.Deserialize(typeof(TestEvent), payload);
        }

        // Assert
        Assert.Throws<JsonException>(Act);
    }

    [Fact]
    public void Deserialize_WithIncorrectTargetType_ThrowsInvalidOperationException()
    {
        // Arrange
        var payload = "{}";

        // Act
        void Act()
        {
            _sut.Deserialize(typeof(object), payload);
        }

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
    }
}