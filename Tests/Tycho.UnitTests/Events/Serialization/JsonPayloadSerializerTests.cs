using System.Text.Json;
using Tycho.Events;
using Tycho.Events.Serialization;
using Tycho.UnitTests._Data.Events;

namespace Tycho.UnitTests.Events.Serialization;

public class JsonPayloadSerializerTests
{
    private static readonly JsonSerializerOptions s_jsonSerializerOptions = new();

    private readonly JsonPayloadSerializer _sut = new();

    [Fact]
    public void Serialize_WithValidEventData_ReturnsSerialized()
    {
        // Arrange
        var eventData = new TestEventWithData();
        string expectedPayload = GetSerializedPayload(eventData)!;

        // Act
        string payload = _sut.Serialize(eventData);

        // Assert
        Assert.Equal(expectedPayload, payload);
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
        string payload = GetSerializedPayload(expectedEventData)!;

        // Act
        TestEventWithData result = _sut.Deserialize<TestEventWithData>(payload);

        // Assert
        Assert.True(expectedEventData.EqualsEvent(result));
    }

    [Fact]
    public void Deserialize_WithMissingProperties_ThrowsJsonException()
    {
        // Arrange
        string payload = "{}";

        // Act
        void Act()
        {
            _sut.Deserialize<TestEventWithRequiredData>(payload);
        }

        // Assert
        Assert.Throws<JsonException>(Act);
    }

    [Fact]
    public void Deserialize_WithInvalidFormat_ThrowsJsonException()
    {
        // Arrange
        string payload = "{property='invalidFormat'}";

        // Act
        void Act()
        {
            _sut.Deserialize<TestEvent>(payload);
        }

        // Assert
        Assert.Throws<JsonException>(Act);
    }

    private static string GetSerializedPayload(object toSerialize)
    {
        return JsonSerializer.Serialize(toSerialize, toSerialize.GetType(), s_jsonSerializerOptions);
    }
}
