using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Utils;

public sealed class ImmutableEquatableArrayTests
{
    [Fact]
    public void EmptyAndNullConversions_ReturnSharedEmptyInstance()
    {
        IEnumerable<string>? nullValues = null;

        ImmutableEquatableArray<string> empty = ImmutableEquatableArray.Empty<string>();
        ImmutableEquatableArray<string> convertedNull = nullValues.ToImmutableEquatableArray();
        ImmutableEquatableArray<string> convertedEmpty = Array.Empty<string>().ToImmutableEquatableArray();

        Assert.Same(empty, convertedNull);
        Assert.Empty(convertedEmpty);
        Assert.Equal(empty, convertedEmpty);
    }

    [Fact]
    public void Constructors_ExposeArrayAndEnumerableValuesThroughReadOnlyIndexing()
    {
        string[] source = ["first", "second"];
        var fromArray = new ImmutableEquatableArray<string>(source);
        var fromEnumerable = new ImmutableEquatableArray<string>((IEnumerable<string>)source);

        Assert.Equal("first", fromArray[0]);
        Assert.Equal("first", fromEnumerable[0]);
        Assert.Equal(2, fromArray.Count);
        Assert.Throws<IndexOutOfRangeException>(() => _ = fromArray[2]);
    }

    [Fact]
    public void EqualityAndHashCode_CompareValuesAndHandleNullAndOtherObjects()
    {
        var first = new ImmutableEquatableArray<string>(["one", "two"]);
        var equal = new ImmutableEquatableArray<string>(["one", "two"]);
        var different = new ImmutableEquatableArray<string>(["two", "one"]);

        Assert.True(first.Equals(equal));
        Assert.True(first.Equals((object)equal));
        Assert.False(first.Equals((ImmutableEquatableArray<string>)null!));
        Assert.False(first.Equals(new object()));
        Assert.False(first.Equals(different));
        Assert.Equal(first.GetHashCode(), equal.GetHashCode());
    }

    [Fact]
    public void StructEnumerator_VisitsEachValueAndStopsAfterLastValue()
    {
        ImmutableEquatableArray<string>.Enumerator enumerator =
            new ImmutableEquatableArray<string>(["one", "two"]).GetEnumerator();

        Assert.True(enumerator.MoveNext());
        Assert.Equal("one", enumerator.Current);
        Assert.True(enumerator.MoveNext());
        Assert.Equal("two", enumerator.Current);
        Assert.False(enumerator.MoveNext());
    }

    private static readonly string[] s_expected = ["one", "two"];

    [Fact]
    public void GenericAndNonGenericEnumerableInterfaces_EnumerateValues()
    {
        var values = new ImmutableEquatableArray<string>(["one", "two"]);

        Assert.Equal(s_expected, ((IEnumerable<string>)values).ToArray());
        Assert.Equal(new object[] { "one", "two" }, [.. ((System.Collections.IEnumerable)values).Cast<object>()]);
    }
}
