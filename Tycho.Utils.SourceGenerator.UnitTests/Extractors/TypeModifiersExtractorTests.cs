using Tycho.Utils.SourceGenerator.Extractors;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Extractors;

public sealed class TypeModifiersExtractorTests
{
    [Theory]
    [InlineData("public static class StaticType { }", "StaticType", "public", "static", null)]
    [InlineData("public abstract class AbstractType { }", "AbstractType", "public", "abstract", null)]
    [InlineData("file class FileType { }", "FileType", "file", null, null)]
    [InlineData("public readonly ref struct ReadonlyRefStruct { }", "ReadonlyRefStruct", "public", "readonly", "ref")]
    [InlineData("public unsafe class UnsafeType { }", "UnsafeType", "public", "unsafe", null)]
    [InlineData("public partial class PartialType { } public partial class PartialType { }", "PartialType", "public", "partial", null)]
    public void TypeModifiersExtractor_CollectsDeclaredModifiersOnce(
        string declaration, string typeName, string first, string? second, string? third)
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create($"namespace Demo; {declaration}");

        // Act
        ImmutableEquatableArray<TypeModifier> result = TypeModifiersExtractor.Extract(
            compilation.DeclaredType(typeName), compilation.Context);

        // Assert
        Assert.Equal(third is not null ? 3 : second is not null ? 2 : 1, result.Count);
        Assert.Equal(first, result[0].Keyword);

        if (second is not null)
        {
            Assert.Equal(second, result[1].Keyword);
        }

        if (third is not null)
        {
            Assert.Equal(third, result[2].Keyword);
        }
    }

    [Theory]
    [InlineData("protected sealed class ProtectedNested { }", "ProtectedNested", "protected", "sealed")]
    [InlineData("private static class PrivateNested { }", "PrivateNested", "private", "static")]
    [InlineData("public new class NewNested { }", "NewNested", "public", "new")]
    [InlineData("protected internal class ProtectedInternalNested { }", "ProtectedInternalNested", "protected", "internal")]
    [InlineData("private protected class PrivateProtectedNested { }", "PrivateProtectedNested", "private", "protected")]
    [InlineData("public delegate void NestedCallback();", "NestedCallback", "public", null)]
    public void TypeModifiersExtractor_CollectsModifiersFromNestedTypesAndDelegates(
        string nestedDeclaration, string nestedName, string first, string? second)
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            $"namespace Demo; public abstract class Outer {{ {nestedDeclaration} }}");

        // Act
        ImmutableEquatableArray<TypeModifier> result = TypeModifiersExtractor.Extract(
            compilation.Type($"Demo.Outer+{nestedName}"), compilation.Context);

        // Assert
        Assert.Equal(second is null ? 1 : 2, result.Count);
        Assert.Equal(first, result[0].Keyword);

        if (second is not null)
        {
            Assert.Equal(second, result[1].Keyword);
        }
    }

    [Fact]
    public void TypeModifiersExtractor_ReturnsEmptyForTypeWithoutExplicitModifiers()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            "namespace Demo; class ImplicitlyInternalType { }");

        // Act
        ImmutableEquatableArray<TypeModifier> result = TypeModifiersExtractor.Extract(
            compilation.Type("Demo.ImplicitlyInternalType"), compilation.Context);

        // Assert
        Assert.Empty(result);
    }
}
