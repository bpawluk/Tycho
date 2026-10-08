using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Models.System;

public sealed class GeneratedTypeModelTests
{
    [Fact]
    public void Constructor_FromTypeDefinition_BuildsGeneratedTypeNames()
    {
        // Arrange
        TypeDefinitionModel owner = ModelHelpers.TypeDefinition("Owner", typeNamespace: "Example");

        // Act
        var sut = new GeneratedTypeModel(owner, "OwnerSetup");

        // Assert
        Assert.Equal("OwnerSetup", sut.Identifier);
        Assert.Equal("OwnerSetup", sut.TypeReference.Name);
        Assert.Equal("Example", sut.TypeReference.Namespace);
        Assert.Equal("OwnerSetup", sut.DeclarationName);
        Assert.Equal("OwnerSetup", sut.ReferenceName);
        Assert.Equal("global::Example.OwnerSetup", sut.FullReferenceName);
    }

    [Fact]
    public void Constructor_FromTypeReference_PreservesOwnerContext()
    {
        // Arrange
        TypeReferenceModel containingType = ModelHelpers.TypeReference("Container", "Example");
        var owner = new TypeReferenceModel("Example", ModelHelpers.Items(containingType), "Owner", ModelHelpers.Items(new TypeArgumentModel("T", ModelHelpers.TypeReference("Value"))));

        // Act
        var sut = new GeneratedTypeModel(owner, "OwnerPublisher");

        // Assert
        Assert.Equal("OwnerPublisher", sut.Identifier);
        Assert.Equal("Example", sut.TypeReference.Namespace);
        Assert.Equal(containingType, Assert.Single(sut.TypeReference.ContainingTypes));
        Assert.Equal("OwnerPublisher", sut.TypeReference.Name);
        TypeArgumentModel expectedArgument = Assert.Single(owner.TypeArguments);
        Assert.Equal(expectedArgument, Assert.Single(sut.TypeReference.TypeArguments));
        Assert.Equal("OwnerPublisher<global::Example.Value>", sut.DeclarationName);
        Assert.Equal(sut.DeclarationName, sut.ReferenceName);
        Assert.Equal("global::Example.Container.OwnerPublisher<global::Example.Value>", sut.FullReferenceName);
    }

    [Fact]
    public void Constructor_NullIdentifier_UsesEmptyIdentifier()
    {
        // Arrange
        TypeReferenceModel owner = ModelHelpers.TypeReference("Owner");

        // Act
        var sut = new GeneratedTypeModel(owner, null);

        // Assert
        Assert.Empty(sut.Identifier);
        Assert.Empty(sut.DeclarationName);
        Assert.Empty(sut.ReferenceName);
        Assert.Equal("global::Example.", sut.FullReferenceName);
    }
}
