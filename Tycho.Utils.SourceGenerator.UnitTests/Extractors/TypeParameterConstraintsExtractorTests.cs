using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Extractors;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Extractors;

public sealed class TypeParameterConstraintsExtractorTests
{
    [Theory]
    [InlineData("class", "class", null)]
    [InlineData("class?", "class?", null)]
    [InlineData("struct", "struct", null)]
    [InlineData("unmanaged", "unmanaged", null)]
    [InlineData("notnull", "notnull", null)]
    [InlineData("IDisposable, new()", "global::System.IDisposable", "new()")]
    [InlineData("IDisposable", "global::System.IDisposable", null)]
    [InlineData("allows ref struct", "allows ref struct", null)]
    public void TypeParameterConstraintsExtractor_ExtractsConstraintKinds(
        string constraint, string first, string? second)
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            $"using System; namespace Demo; public class ConstrainedType<T> where T : {constraint} {{ }}");
        ITypeParameterSymbol parameter = compilation.TypeParameter("Demo.ConstrainedType`1", "T");

        // Act
        ImmutableEquatableArray<TypeParameterConstraintModel> result = TypeParameterConstraintsExtractor.Extract(
            parameter, compilation.Context);

        // Assert
        Assert.Equal(second is null ? 1 : 2, result.Count);
        Assert.Equal(first, result[0].Keyword);
        if (second is not null)
        {
            Assert.Equal(second, result[1].Keyword);
        }
    }
}
