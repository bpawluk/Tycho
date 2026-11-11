using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.System;

public sealed class TypeArgumentModelTests
{
    [Fact]
    public void Constructor_SetsNameAndValue()
    {
        // Arrange
        TypeReferenceModel value = ModelHelpers.TypeReference("Value");

        // Act
        var sut = new TypeArgumentModel("T", value);

        // Assert
        Assert.Equal("T", sut.Name);
        Assert.Equal(value, sut.Value);
    }

    [Fact]
    public void Constructor_NullName_UsesEmptyName()
    {
        // Arrange
        TypeReferenceModel value = ModelHelpers.TypeReference("Value");

        // Act
        var sut = new TypeArgumentModel(null, value);

        // Assert
        Assert.Empty(sut.Name);
        Assert.Equal(value, sut.Value);
    }

    [Fact]
    public void Equality_EquivalentAndDifferentArguments_UsesNameAndValue()
    {
        // Arrange
        var first = new TypeArgumentModel("T", ModelHelpers.TypeReference("Value"));
        var equal = new TypeArgumentModel("T", ModelHelpers.TypeReference("Value"));
        var different = new TypeArgumentModel("U", ModelHelpers.TypeReference("Value"));

        // Act
        bool equalWithOperator = first == equal;
        bool unequalWithOperator = first != different;

        // Assert
        ModelHelpers.AssertValueSemantics(first, equal, different);
        Assert.True(equalWithOperator);
        Assert.True(unequalWithOperator);
        Assert.False(first != equal);
        Assert.False(first == different);
    }

    [Fact]
    public void Matches_UsesNameAndValue()
    {
        // Arrange
        var first = new TypeArgumentModel("T", ModelHelpers.TypeReference("Value"));
        var equal = new TypeArgumentModel("T", ModelHelpers.TypeReference("Value"));
        var different = new TypeArgumentModel("U", ModelHelpers.TypeReference("Value"));

        // Act
        bool matchesEquivalent = first.Matches(equal);
        bool matchesDifferent = first.Matches(different);

        // Assert
        Assert.True(matchesEquivalent);
        Assert.False(matchesDifferent);
    }
}
