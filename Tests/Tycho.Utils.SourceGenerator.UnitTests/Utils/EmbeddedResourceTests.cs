using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Utils;

public sealed class EmbeddedResourceTests
{
    [Fact]
    public void GetContent_WhenResourceIsMissing_IncludesNormalizedResourceName()
    {
        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => EmbeddedResource.GetContent("Missing/Resource.txt"));

        // Assert
        Assert.Contains("Missing.Resource.txt", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GetContent_WhenResourceIsMissing_IncludesAssemblyName()
    {
        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => EmbeddedResource.GetContent("Missing/Resource.txt"));

        // Assert
        Assert.Contains("Tycho.Utils.SourceGenerator", exception.Message, StringComparison.Ordinal);
    }
}
