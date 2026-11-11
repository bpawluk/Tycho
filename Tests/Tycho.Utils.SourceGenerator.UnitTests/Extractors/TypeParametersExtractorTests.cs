using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Extractors;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Extractors;

public sealed class TypeParametersExtractorTests
{
    [Fact]
    public void TypeParametersExtractor_ReturnsParameterForGenericNamedType()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            "namespace Demo; public class GenericType<T> { }");

        // Act
        ImmutableEquatableArray<TypeParameterModel> result = TypeParametersExtractor.Extract(
            compilation.Type("Demo.GenericType`1"), compilation.Context);

        // Assert
        TypeParameterModel item = Assert.Single(result);
        Assert.Equal("T", item.Name);
    }

    [Fact]
    public void TypeParametersExtractor_ReturnsEmptyForNonGenericNamedType()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            "namespace Demo; public class PlainType { }");

        // Act
        ImmutableEquatableArray<TypeParameterModel> result = TypeParametersExtractor.Extract(
            compilation.Type("Demo.PlainType"), compilation.Context);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void TypeParametersExtractor_ReturnsEmptyForTypeParameterSymbol()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            "namespace Demo; public class GenericType<T> { }");
        ITypeParameterSymbol typeParameter = compilation.TypeParameter("Demo.GenericType`1", "T");

        // Act
        ImmutableEquatableArray<TypeParameterModel> result = TypeParametersExtractor.Extract(
            typeParameter, compilation.Context);

        // Assert
        Assert.Empty(result);
    }
}
