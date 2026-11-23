using Tycho.Requests;

namespace Tycho.UnitTests.Requests;

public sealed class NoResponseTests
{
    [Fact]
    public void Equality_AllValues_AreEqual()
    {
        // Arrange
        NoResponse first = NoResponse.Value;
        NoResponse second = default;

        // Act & Assert
        Assert.True(first.Equals(second));
        Assert.True(first == second);
        Assert.False(first != second);
        Assert.True(first.Equals((object)second));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData("()")]
    public void Equals_WithOtherObject_ReturnsFalse(object? other)
    {
        // Act & Assert
        Assert.False(NoResponse.Value.Equals(other));
    }

    [Fact]
    public void GetHashCode_EqualValues_WorkAsDictionaryKeys()
    {
        // Arrange
        NoResponse first = NoResponse.Value;
        NoResponse second = default;
        var values = new Dictionary<NoResponse, string> { [first] = "result" };

        // Act & Assert
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.Equal("result", values[second]);
    }

    [Fact]
    public void ToString_ReturnsUnitRepresentation()
    {
        // Act & Assert
        Assert.Equal("()", NoResponse.Value.ToString());
    }
}
