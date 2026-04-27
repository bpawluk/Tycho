using System.Text.Json;
using Tycho.Events;
using Tycho.Persistence.EFCore.Serialization;
using Tycho.Persistence.EFCore.UnitTests._Data.Events;

namespace Tycho.Persistence.EFCore.UnitTests.Serialization;

public class JsonPayloadSerializerTests
{
    private readonly JsonPayloadSerializer _sut = new();

    public static IEnumerable<object?[]> NonStrings =>
    [
        [null],
        [123],
        [new object()]
    ];

    [Fact]
    public void Serialize_WithValidEventData_ReturnsSerialized()
    {
        // Arrange
        var eventData = new TestEventWithData();
        var expectedPayload = GetSerializedPayload(eventData)!;

        // Act
        var payload = _sut.Serialize(eventData);

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
        var payload = GetSerializedPayload(expectedEventData)!;

        // Act
        var result = _sut.Deserialize<TestEventWithData>(payload);

        // Assert
        Assert.True(expectedEventData.EqualsEvent(result));
    }


    [Theory]
    [MemberData(nameof(NonStrings))]
    public void Deserialize_WithNonString_ThrowsArgumentException(object? payload)
    {
        // Arrange
        // - no arrangement required

        // Act
        void Act()
        {
            _sut.Deserialize<TestEvent>(payload!);
        }

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Deserialize_WithMissingProperties_ThrowsJsonException()
    {
        // Arrange
        var payload = "{}";

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
        var payload = "{property='invalidFormat'}";

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
        return JsonSerializer.Serialize(toSerialize, toSerialize.GetType(), new JsonSerializerOptions());
    }
}