using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Utils;

public sealed class ImmutableEquatableArrayOfTTests
{
    [Fact]
    public void ArrayConstructor_ExposesValuesByIndex()
    {
        // Arrange
        string[] values = ["first", "second"];

        // Act
        var result = new ImmutableEquatableArray<string>(values);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("first", result[0]);
        Assert.Equal("second", result[1]);
    }

    [Fact]
    public void EnumerableConstructor_ExposesValuesByIndex()
    {
        // Arrange
        IEnumerable<string> values = ["first", "second"];

        // Act
        var result = new ImmutableEquatableArray<string>(values);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("first", result[0]);
        Assert.Equal("second", result[1]);
    }

    [Fact]
    public void Indexer_ThrowsForOutOfRangeIndex()
    {
        // Arrange
        var values = new ImmutableEquatableArray<string>(["first", "second"]);

        // Act and assert
        Assert.Throws<IndexOutOfRangeException>(() => _ = values[2]);
    }

    [Fact]
    public void Equals_ReturnsTrueForEqualValues()
    {
        // Arrange
        var first = new ImmutableEquatableArray<string>(["one", "two"]);
        var equal = new ImmutableEquatableArray<string>(["one", "two"]);

        // Assert
        Assert.True(first.Equals(equal));
        Assert.True(first.Equals((object)equal));
    }

    [Fact]
    public void Equals_ReturnsFalseForDifferentValueOrder()
    {
        // Arrange
        var first = new ImmutableEquatableArray<string>(["one", "two"]);
        var different = new ImmutableEquatableArray<string>(["two", "one"]);

        // Assert
        Assert.False(first.Equals(different));
    }

    [Fact]
    public void Equals_ReturnsFalseForNull()
    {
        // Arrange
        var values = new ImmutableEquatableArray<string>(["one", "two"]);

        // Assert
        Assert.False(values.Equals((ImmutableEquatableArray<string>)null!));
    }

    [Fact]
    public void Equals_ReturnsFalseForOtherObject()
    {
        // Arrange
        var values = new ImmutableEquatableArray<string>(["one", "two"]);

        // Assert
        Assert.False(values.Equals(new object()));
    }

    [Fact]
    public void GetHashCode_EqualValuesProduceSameHashCode()
    {
        // Arrange
        var first = new ImmutableEquatableArray<string>(["one", "two"]);
        var equal = new ImmutableEquatableArray<string>(["one", "two"]);

        // Assert
        Assert.Equal(first.GetHashCode(), equal.GetHashCode());
    }

    [Fact]
    public void StructEnumerator_VisitsEachValueAndStopsAfterLastValue()
    {
        // Arrange
        ImmutableEquatableArray<string>.Enumerator enumerator =
            new ImmutableEquatableArray<string>(["one", "two"]).GetEnumerator();

        // Assert
        Assert.True(enumerator.MoveNext());
        Assert.Equal("one", enumerator.Current);
        Assert.True(enumerator.MoveNext());
        Assert.Equal("two", enumerator.Current);
        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public void GenericEnumerableInterface_EnumeratesValues()
    {
        // Arrange
        var values = new ImmutableEquatableArray<string>(["one", "two"]);

        // Act
        string[] result = [.. (IEnumerable<string>)values];

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal("one", result[0]);
        Assert.Equal("two", result[1]);
    }

    [Fact]
    public void NonGenericEnumerableInterface_EnumeratesValues()
    {
        // Arrange
        var values = new ImmutableEquatableArray<string>(["one", "two"]);

        // Act
        System.Collections.IEnumerator result = ((System.Collections.IEnumerable)values).GetEnumerator();

        // Assert
        Assert.True(result.MoveNext());
        Assert.Equal("one", result.Current);
        Assert.True(result.MoveNext());
        Assert.Equal("two", result.Current);
        Assert.False(result.MoveNext());
    }
}
