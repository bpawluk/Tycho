using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Extractors;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Extractors;

public sealed class TypeReferenceModelExtractorTests
{
    [Fact]
    public void TypeReferenceModelExtractor_ExtractsTypeParameter()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            "namespace Demo; public class GenericType<T> { }");
        ITypeParameterSymbol typeParameter = compilation.TypeParameter("Demo.GenericType`1", "T");

        // Act
        TypeReferenceModel result = TypeReferenceModelExtractor.Extract(typeParameter, compilation.Context);

        // Assert
        Assert.True(result.IsTypeParameter);
        Assert.Equal("T", result.FullReferenceName);
    }

    [Fact]
    public void TypeReferenceModelExtractor_ExtractsNestedGenericNamedType()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            "namespace Demo; public class Outer<T> { public class Inner<U> { } }");

        // Act
        TypeReferenceModel result = TypeReferenceModelExtractor.Extract(
            compilation.Type("Demo.Outer`1+Inner`1"), compilation.Context);

        // Assert
        Assert.Equal("global::Demo.Outer<T>.Inner<U>", result.FullReferenceName);
    }

    [Fact]
    public void TypeReferenceModelExtractor_ExtractsArrayType()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            "namespace Demo; public class ArrayContext { }");
        ITypeSymbol array = compilation.Compilation.CreateArrayTypeSymbol(
            compilation.Compilation.GetSpecialType(SpecialType.System_Int32));

        // Act
        TypeReferenceModel result = TypeReferenceModelExtractor.Extract(array, compilation.Context);

        // Assert
        Assert.Equal(array.Name, result.Name);
        Assert.Empty(result.ContainingTypes);
        Assert.Empty(result.TypeArguments);
    }

    [Fact]
    public void TypeReferenceModelExtractor_ExtractsTypeInGlobalNamespace()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create("public class GlobalType { }");

        // Act
        TypeReferenceModel result = TypeReferenceModelExtractor.Extract(
            compilation.Type("GlobalType"), compilation.Context);

        // Assert
        Assert.Empty(result.Namespace);
    }
}
