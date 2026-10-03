using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.System;

public sealed class TypeDefinitionModelTests
{
    [Fact]
    public void Constructor_SetsTypeDefinitionMembers()
    {
        // Arrange
        TypeDefinitionModel outer = ModelHelpers.TypeDefinition("Outer", typeNamespace: "Example");
        var parameter = new TypeParameterModel("T", ModelHelpers.Items(TypeParameterConstraintModel.ReferenceType));
        TypeDefinitionModel sut = ModelHelpers.TypeDefinition(
            "Inner",
            containingTypes: ModelHelpers.Items(outer),
            kind: TypeKind.Struct,
            modifiers: ModelHelpers.Items(TypeModifier.Public, TypeModifier.ReadOnly),
            typeParameters: ModelHelpers.Items(parameter));

        // Assert
        Assert.Equal(TypeKind.Struct, sut.Kind);
        Assert.Equal("Example", sut.Namespace);
        Assert.Equal(outer, Assert.Single(sut.ContainingTypes));
        Assert.Equal("Inner", sut.Name);
        Assert.Equal(2, sut.Modifiers.Count);
        Assert.Equal(TypeModifier.Public, sut.Modifiers[0]);
        Assert.Equal(TypeModifier.ReadOnly, sut.Modifiers[1]);
        Assert.Equal(parameter, Assert.Single(sut.TypeParameters));
    }

    [Fact]
    public void ComputedNames_DescribeNestedGenericType()
    {
        // Arrange
        TypeDefinitionModel outer = ModelHelpers.TypeDefinition("Outer", typeNamespace: "Example");
        var parameter = new TypeParameterModel("T", ModelHelpers.Items(TypeParameterConstraintModel.ReferenceType));
        TypeDefinitionModel sut = ModelHelpers.TypeDefinition(
            "Inner",
            containingTypes: ModelHelpers.Items(outer),
            kind: TypeKind.Struct,
            modifiers: ModelHelpers.Items(TypeModifier.Public, TypeModifier.ReadOnly),
            typeParameters: ModelHelpers.Items(parameter));

        // Assert
        Assert.Equal("<T>", sut.TypeParametersSuffix);
        Assert.Equal("Inner<T>", sut.DeclarationName);
        Assert.Equal("Outer.Inner<T>", sut.FullDeclarationName);
        Assert.Equal("Inner`1", sut.MetadataName);
        Assert.Equal("Example.Outer.Inner`1", sut.FullMetadataName);
        Assert.Equal("public readonly struct Inner<T>", sut.DeclarationSignature);
        Assert.Equal("Example.Outer.Inner`1", sut.ToString());
    }

    [Fact]
    public void GetReference_PreservesNestedGenericTypeContext()
    {
        // Arrange
        TypeDefinitionModel outer = ModelHelpers.TypeDefinition("Outer", typeNamespace: "Example");
        var parameter = new TypeParameterModel("T", ModelHelpers.Items(TypeParameterConstraintModel.ReferenceType));
        TypeDefinitionModel sut = ModelHelpers.TypeDefinition(
            "Inner",
            containingTypes: ModelHelpers.Items(outer),
            kind: TypeKind.Struct,
            modifiers: ModelHelpers.Items(TypeModifier.Public, TypeModifier.ReadOnly),
            typeParameters: ModelHelpers.Items(parameter));

        // Act
        TypeReferenceModel reference = sut.GetReference();

        // Assert
        Assert.Equal("Example", reference.Namespace);
        Assert.Equal("Inner", reference.Name);
        Assert.Equal("global::Example.Outer.Inner<T>", reference.FullReferenceName);
        Assert.True(Assert.Single(reference.TypeArguments).Value.IsTypeParameter);
    }

    [Fact]
    public void Constructor_WithoutOptionalCollections_UsesEmptyCollections()
    {
        // Arrange
        var sut = new TypeDefinitionModel("", null, TypeKind.Class, null, "Widget", null);

        // Act
        string declaration = sut.DeclarationSignature;

        // Assert
        Assert.Empty(sut.Namespace);
        Assert.Empty(sut.ContainingTypes);
        Assert.Empty(sut.Modifiers);
        Assert.Empty(sut.TypeParameters);
        Assert.Equal(string.Empty, sut.TypeParametersSuffix);
        Assert.Equal("Widget", sut.DeclarationName);
        Assert.Equal("Widget", sut.FullDeclarationName);
        Assert.Equal("Widget", sut.MetadataName);
        Assert.Equal("Widget", sut.FullMetadataName);
        Assert.Equal("class Widget", declaration);
        Assert.Equal("Widget", sut.ToString());
    }

    [Fact]
    public void Equality_EquivalentAndDifferentDefinitions_UsesAllFields()
    {
        // Arrange
        TypeDefinitionModel first = ModelHelpers.TypeDefinition("Widget");
        TypeDefinitionModel equal = ModelHelpers.TypeDefinition("Widget");
        TypeDefinitionModel different = ModelHelpers.TypeDefinition("Gadget");

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
    public void Equality_DefaultStructUsesEmptyCollections()
    {
        TypeDefinitionModel first = default;
        TypeDefinitionModel equal = default;

        Assert.True(first.Equals(equal));
    }

    [Fact]
    public void Equality_DetectsDifferencesInEachDefinitionField()
    {
        TypeDefinitionModel baseline = ModelHelpers.TypeDefinition("Widget");
        TypeDefinitionModel outer = ModelHelpers.TypeDefinition("Outer");
        TypeParameterModel parameter = new("T", ModelHelpers.Items(TypeParameterConstraintModel.ReferenceType));

        TypeDefinitionModel[] differentDefinitions =
        [
            ModelHelpers.TypeDefinition("Widget", kind: TypeKind.Struct),
            ModelHelpers.TypeDefinition("Widget", typeNamespace: "Other"),
            ModelHelpers.TypeDefinition("Widget", containingTypes: ModelHelpers.Items(outer)),
            ModelHelpers.TypeDefinition("Gadget"),
            ModelHelpers.TypeDefinition("Widget", modifiers: ModelHelpers.Items(TypeModifier.Public)),
            ModelHelpers.TypeDefinition("Widget", typeParameters: ModelHelpers.Items(parameter)),
        ];

        Assert.All(differentDefinitions, different => Assert.False(baseline.Equals(different)));
    }
}
