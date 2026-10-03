using Tycho.Utils.SourceGenerator.Extractors;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Extractors;

public sealed class TychoDefinitionKindExtractorTests
{
    [Fact]
    public void TychoDefinitionKindExtractor_RecognizesDirectAppSubclass()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Tycho.Apps { public class TychoApp { } }
            namespace Demo { public class App : Tycho.Apps.TychoApp { } }
            """);

        // Act
        TychoDefinitionKind result = TychoDefinitionKindExtractor.Extract(
            compilation.Type("Demo.App"), compilation.Context);

        // Assert
        Assert.Equal(TychoDefinitionKind.App, result);
    }

    [Fact]
    public void TychoDefinitionKindExtractor_RecognizesIndirectAppSubclass()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Tycho.Apps { public class TychoApp { } }
            namespace Demo
            {
                public class App : Tycho.Apps.TychoApp { }
                public class AppChild : App { }
            }
            """);

        // Act
        TychoDefinitionKind result = TychoDefinitionKindExtractor.Extract(
            compilation.Type("Demo.AppChild"), compilation.Context);

        // Assert
        Assert.Equal(TychoDefinitionKind.App, result);
    }

    [Fact]
    public void TychoDefinitionKindExtractor_RecognizesDirectModuleSubclass()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Tycho.Modules { public class TychoModule { } }
            namespace Demo { public class Module : Tycho.Modules.TychoModule { } }
            """);

        // Act
        TychoDefinitionKind result = TychoDefinitionKindExtractor.Extract(
            compilation.Type("Demo.Module"), compilation.Context);

        // Assert
        Assert.Equal(TychoDefinitionKind.Module, result);
    }

    [Fact]
    public void TychoDefinitionKindExtractor_ReturnsUnknownForUnrelatedTypeWithTychoBases()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Tycho.Apps { public class TychoApp { } }
            namespace Tycho.Modules { public class TychoModule { } }
            namespace Demo { public class Unrelated { } }
            """);

        // Act
        TychoDefinitionKind result = TychoDefinitionKindExtractor.Extract(
            compilation.Type("Demo.Unrelated"), compilation.Context);

        // Assert
        Assert.Equal(TychoDefinitionKind.Unknown, result);
    }

    [Fact]
    public void TychoDefinitionKindExtractor_ReturnsUnknownWhenTychoBasesAreAbsent()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            "namespace Demo; public class Unrelated { }");

        // Act
        TychoDefinitionKind result = TychoDefinitionKindExtractor.Extract(
            compilation.Type("Demo.Unrelated"), compilation.Context);

        // Assert
        Assert.Equal(TychoDefinitionKind.Unknown, result);
    }
}
