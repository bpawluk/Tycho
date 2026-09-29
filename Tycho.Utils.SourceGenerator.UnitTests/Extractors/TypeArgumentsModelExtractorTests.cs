using Tycho.Utils.SourceGenerator.Extractors;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Extractors;

public sealed class TypeArgumentsModelExtractorTests
{
    [Fact]
    public void TypeArgumentsModelExtractor_ExtractsGenericMethodArgument()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            "namespace Demo; public class Methods { public void GenericMethod<T>() { } }");

        // Act
        ImmutableEquatableArray<TypeArgumentModel> result = TypeArgumentsModelExtractor.Extract(
            compilation.Method("Demo.Methods", "GenericMethod"), compilation.Context);

        // Assert
        TypeArgumentModel item = Assert.Single(result);
        Assert.Equal("T", item.Name);
    }

    [Fact]
    public void TypeArgumentsModelExtractor_ReturnsEmptyForNonGenericMethod()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            "namespace Demo; public class Methods { public void NonGenericMethod() { } }");

        // Act
        ImmutableEquatableArray<TypeArgumentModel> result = TypeArgumentsModelExtractor.Extract(
            compilation.Method("Demo.Methods", "NonGenericMethod"), compilation.Context);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void TypeArgumentsModelExtractor_ExtractsGenericTypeArgument()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            "namespace Demo; public class GenericType<T> { }");

        // Act
        ImmutableEquatableArray<TypeArgumentModel> result = TypeArgumentsModelExtractor.Extract(
            compilation.Type("Demo.GenericType`1"), compilation.Context);

        // Assert
        TypeArgumentModel item = Assert.Single(result);
        Assert.Equal("T", item.Name);
    }

    [Fact]
    public void TypeArgumentsModelExtractor_ReturnsEmptyForNonGenericType()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            "namespace Demo; public class NonGenericType { }");

        // Act
        ImmutableEquatableArray<TypeArgumentModel> result = TypeArgumentsModelExtractor.Extract(
            compilation.Type("Demo.NonGenericType"), compilation.Context);

        // Assert
        Assert.Empty(result);
    }
}
