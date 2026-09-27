using Tycho.Utils.SourceGenerator.TemplateModels;

namespace Tycho.Utils.SourceGenerator.UnitTests.TemplateModels;

public sealed class ContainingTypeTMTests
{
    [Fact]
    public void Constructor_NullConstraints_UsesEmptyArray()
    {
        var sut = new ContainingTypeTM("class Outer<T>", null!);

        Assert.Equal("class Outer<T>", sut.Declaration);
        Assert.Empty(sut.Constraints);
    }
}
