using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.System;

public sealed class TypeReferenceModelTests
{
    [Fact]
    public void Constructor_SimpleType_SetsPropertiesAndNames()
    {
        // Arrange
        var sut = new TypeReferenceModel("Example", "Widget");

        // Act
        string referenceName = sut.ReferenceName;
        string fullReferenceName = sut.FullReferenceName;

        // Assert
        Assert.Equal("Example", sut.Namespace);
        Assert.False(sut.IsTypeParameter);
        Assert.Empty(sut.ContainingTypes);
        Assert.Equal("Widget", sut.Name);
        Assert.Empty(sut.TypeArguments);
        Assert.Equal(string.Empty, sut.TypeArgumentsSuffix);
        Assert.Equal("Widget", referenceName);
        Assert.Equal("global::Example.Widget", fullReferenceName);
        Assert.Equal(fullReferenceName, sut.ToString());
    }

    [Fact]
    public void FullReferenceName_NestedGenericType_UsesQualifiedPath()
    {
        // Arrange
        var outer = new TypeReferenceModel("Example", "Outer");
        var argument = new TypeArgumentModel("T", ModelHelpers.TypeReference("Value", "Values"));
        var sut = new TypeReferenceModel(
            "Example",
            ModelHelpers.Items(outer),
            "Inner",
            ModelHelpers.Items(argument));

        // Act
        string result = sut.FullReferenceName;

        // Assert
        Assert.Equal("Example", sut.Namespace);
        Assert.False(sut.IsTypeParameter);
        Assert.Equal(outer, sut.ContainingTypes[0]);
        Assert.Equal("Inner", sut.Name);
        Assert.Equal(argument, sut.TypeArguments[0]);
        Assert.Equal("<global::Values.Value>", sut.TypeArgumentsSuffix);
        Assert.Equal("Inner<global::Values.Value>", sut.ReferenceName);
        Assert.Equal("global::Example.Outer.Inner<global::Values.Value>", result);
    }

    [Fact]
    public void TypeParameter_UsesUnqualifiedReference()
    {
        // Arrange
        TypeReferenceModel sut = TypeReferenceModel.TypeParameter("Example", "TValue");

        // Act
        string result = sut.FullReferenceName;

        // Assert
        Assert.Equal("Example", sut.Namespace);
        Assert.True(sut.IsTypeParameter);
        Assert.Empty(sut.ContainingTypes);
        Assert.Equal("TValue", sut.Name);
        Assert.Empty(sut.TypeArguments);
        Assert.Equal("TValue", sut.ReferenceName);
        Assert.Equal("TValue", result);
    }

    [Fact]
    public void Constructor_NullCollectionsAndNames_UsesEmptyValues()
    {
        // Arrange
        var sut = new TypeReferenceModel(null, null, null, null);

        // Act
        string result = sut.FullReferenceName;

        // Assert
        Assert.Equal(string.Empty, sut.Namespace);
        Assert.Equal(string.Empty, sut.Name);
        Assert.Empty(sut.ContainingTypes);
        Assert.Empty(sut.TypeArguments);
        Assert.Equal("global::", result);
    }

    [Fact]
    public void Matches_EquivalentAndDifferentReferences_ReturnsExpectedResult()
    {
        // Arrange
        var first = new TypeReferenceModel("Example", "Widget");
        var equal = new TypeReferenceModel("Example", "Widget");
        var different = new TypeReferenceModel("Other", "Widget");

        // Act
        bool matchesEquivalent = first.Matches(equal);
        bool matchesDifferent = first.Matches(different);

        // Assert
        Assert.True(matchesEquivalent);
        Assert.False(matchesDifferent);
    }

    [Fact]
    public void Matches_DetectsDifferencesInTypeParameterContainingTypeNameAndArguments()
    {
        TypeReferenceModel plain = new("Example", "Widget");
        TypeReferenceModel typeParameter = TypeReferenceModel.TypeParameter("Example", "Widget");
        TypeReferenceModel containingType = new("Example", ModelHelpers.Items(new TypeReferenceModel("Example", "Outer")), "Widget", null);
        TypeReferenceModel differentName = new("Example", "Gadget");
        TypeReferenceModel generic = new(
            "Example",
            ImmutableEquatableArray<TypeReferenceModel>.Empty,
            "Widget",
            ModelHelpers.Items(new TypeArgumentModel("T", ModelHelpers.TypeReference("Value"))));
        TypeReferenceModel differentArgument = new(
            "Example",
            ImmutableEquatableArray<TypeReferenceModel>.Empty,
            "Widget",
            ModelHelpers.Items(new TypeArgumentModel("T", ModelHelpers.TypeReference("OtherValue"))));

        Assert.False(plain.Matches(typeParameter));
        Assert.False(plain.Matches(containingType));
        Assert.False(plain.Matches(differentName));
        Assert.False(plain.Matches(generic));
        Assert.False(generic.Matches(differentArgument));
    }

    [Fact]
    public void Equality_DefaultStructUsesEmptyCollections()
    {
        TypeReferenceModel first = default;
        TypeReferenceModel equal = default;

        Assert.True(first.Equals(equal));
    }

    [Fact]
    public void Equality_DetectsDifferencesInEachReferenceField()
    {
        TypeReferenceModel baseline = new("Example", "Widget");
        TypeReferenceModel differentTypeParameter = TypeReferenceModel.TypeParameter("Example", "Widget");
        TypeReferenceModel differentContainingType = new(
            "Example",
            ModelHelpers.Items(new TypeReferenceModel("Example", "Outer")),
            "Widget",
            ImmutableEquatableArray<TypeArgumentModel>.Empty);
        TypeReferenceModel differentName = new("Example", "Gadget");
        TypeReferenceModel differentArguments = new(
            "Example",
            ImmutableEquatableArray<TypeReferenceModel>.Empty,
            "Widget",
            ModelHelpers.Items(new TypeArgumentModel("T", ModelHelpers.TypeReference("Value"))));

        Assert.False(baseline.Equals(differentTypeParameter));
        Assert.False(baseline.Equals(differentContainingType));
        Assert.False(baseline.Equals(differentName));
        Assert.False(baseline.Equals(differentArguments));
    }

    [Fact]
    public void Equality_EquivalentAndDifferentReferences_UsesAllFields()
    {
        // Arrange
        var first = new TypeReferenceModel("Example", "Widget");
        var equal = new TypeReferenceModel("Example", "Widget");
        var different = new TypeReferenceModel("Other", "Widget");

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
}
