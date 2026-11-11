using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Extractors;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;
using TypeKind = Tycho.Utils.SourceGenerator.Models.System.TypeKind;

namespace Tycho.Utils.SourceGenerator.UnitTests.Extractors;

public sealed class TypeKindExtractorTests
{
    [Theory]
    [InlineData("public class PlainType { }", "Demo.PlainType", "class")]
    [InlineData("public record RecordClass { }", "Demo.RecordClass", "record class")]
    [InlineData("public struct PlainStruct { }", "Demo.PlainStruct", "struct")]
    [InlineData("public record struct RecordStruct { }", "Demo.RecordStruct", "record struct")]
    [InlineData("public interface IContract { }", "Demo.IContract", "interface")]
    [InlineData("public enum State { Ready }", "Demo.State", "enum")]
    [InlineData("public delegate void Callback();", "Demo.Callback", "other")]
    public void TypeKindExtractor_MapsTypeDeclarations(string declaration, string metadataName, string expected)
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create($"namespace Demo; {declaration}");

        // Act
        TypeKind result = TypeKindExtractor.Extract(compilation.Type(metadataName), compilation.Context);

        // Assert
        Assert.Equal(expected, result.Keyword);
    }

    [Fact]
    public void TypeKindExtractor_MapsTypeParametersToOther()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            "namespace Demo; public class GenericType<T> { }");
        ITypeParameterSymbol typeParameter = compilation.TypeParameter("Demo.GenericType`1", "T");

        // Act
        TypeKind result = TypeKindExtractor.Extract(typeParameter, compilation.Context);

        // Assert
        Assert.Equal(TypeKind.Other, result);
    }
}
