using Microsoft.Extensions.DependencyInjection;
using Tycho.TestUtils.Modules;
using Tycho.Utils;

namespace Tycho.UnitTests.Utils;

public sealed class GeneratedSetupExtensionsTests
{
    [Fact]
    public void AddGeneratedSetup_WhenSetupTypeIsMissing_ReportsMissingDefinitionSetup()
    {
        // Arrange
        var module = new ModuleWithoutSetup();
        var services = new ServiceCollection();

        // Act
        NotImplementedException exception = Assert.Throws<NotImplementedException>(
            () => module.AddGeneratedSetup(services));

        // Assert
        Assert.Contains(nameof(ModuleWithoutSetup), exception.Message);
        Assert.Contains(nameof(TychoDefinitionAttribute), exception.Message);
    }

    [Fact]
    public void AddGeneratedSetup_WhenSetupMethodIsMissing_IdentifiesSetupType()
    {
        // Arrange
        var module = new ModuleWithoutSetupMethod();
        var services = new ServiceCollection();

        // Act
        NotImplementedException exception = Assert.Throws<NotImplementedException>(
            () => module.AddGeneratedSetup(services));

        // Assert
        Assert.Contains(nameof(ModuleWithoutSetupMethodSetup), exception.Message);
        Assert.Contains("public static 'Setup' method", exception.Message);
    }

    [Fact]
    public void AddGeneratedSetup_WhenSetupThrows_PreservesOriginalCause()
    {
        // Arrange
        var module = new ModuleWithThrowingSetup();
        var services = new ServiceCollection();

        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => module.AddGeneratedSetup(services));

        // Assert
        Assert.Contains(nameof(ModuleWithThrowingSetup), exception.Message);
        FormatException cause = Assert.IsType<FormatException>(exception.InnerException);
        Assert.Equal("Invalid generated registration.", cause.Message);
    }

    private sealed class ModuleWithoutSetup : TestModule;

    private sealed class ModuleWithoutSetupMethod : TestModule;

    private static class ModuleWithoutSetupMethodSetup;

    private sealed class ModuleWithThrowingSetup : TestModule;

    private static class ModuleWithThrowingSetupSetup
    {
        public static void Setup(IServiceCollection services) => throw new FormatException("Invalid generated registration.");
    }
}
