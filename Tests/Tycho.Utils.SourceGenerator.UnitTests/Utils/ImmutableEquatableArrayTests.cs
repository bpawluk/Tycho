using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Utils;

public sealed class ImmutableEquatableArrayTests
{
    [Fact]
    public void Empty_ReturnsSharedEmptyInstance()
    {
        // Act
        ImmutableEquatableArray<string> result = ImmutableEquatableArray.Empty<string>();

        // Assert
        Assert.Same(ImmutableEquatableArray<string>.Empty, result);
        Assert.Empty(result);
    }

    [Fact]
    public void ToImmutableEquatableArray_NullValuesReturnSharedEmptyInstance()
    {
        // Arrange
        IEnumerable<string>? values = null;

        // Act
        ImmutableEquatableArray<string> result = values.ToImmutableEquatableArray();

        // Assert
        Assert.Same(ImmutableEquatableArray<string>.Empty, result);
    }

    [Fact]
    public void ToImmutableEquatableArray_EmptyValuesReturnEmptyArray()
    {
        // Act
        ImmutableEquatableArray<string> result = Array.Empty<string>().ToImmutableEquatableArray();

        // Assert
        Assert.Empty(result);
        Assert.Equal(ImmutableEquatableArray<string>.Empty, result);
    }
}
