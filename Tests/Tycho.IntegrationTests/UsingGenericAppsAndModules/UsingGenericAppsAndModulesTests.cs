using Microsoft.CodeAnalysis;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.UsingGenericAppsAndModules;

public sealed class UsingGenericAppsAndModulesTests
{
    [Fact]
    public void TychoDoesNotEnableYet_GenericAppDefinitions()
    {
        // Arrange
        string genericApp =
            """
            using Microsoft.Extensions.DependencyInjection;
            using Tycho.Apps;

            namespace Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT;

            [TychoDefinition]
            public partial class TestApp<T> : TychoApp
            {
                protected override void DefineContract(IAppContract app) { }
                protected override void DefineEvents(IAppEvents app) { }
                protected override void IncludeModules(IAppStructure app) { }
                protected override void RegisterServices(IServiceCollection app) { }
            }
            """;

        // Act
        IReadOnlyCollection<Diagnostic> result = CompilationHelpers.CompileWithTychoGenerator(genericApp);

        // Assert
        static bool IsGenericAppCausedError(Diagnostic diagnostic)
        {
            return diagnostic.GetMessage().Contains("'TestApp' does not implement inherited abstract member");
        }

        var compilationErrors = result.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        Assert.NotEmpty(compilationErrors);

        var genericAppErrors = compilationErrors.Where(IsGenericAppCausedError).ToList();
        Assert.NotEmpty(genericAppErrors);

        var otherErrors = compilationErrors.Where(d => !IsGenericAppCausedError(d)).ToList();
        Assert.Empty(otherErrors);
    }

    [Fact]
    public void TychoDoesNotEnableYet_GenericModuleDefinitions()
    {
        // Arrange
        string genericApp =
            """
            using Microsoft.Extensions.DependencyInjection;
            using Tycho.Modules;

            namespace Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT;

            [TychoDefinition]
            public partial class TestModule<T> : TychoModule
            {
                protected override void DefineContract(IModuleContract module) { }
                protected override void DefineEvents(IModuleEvents module) { }
                protected override void IncludeModules(IModuleStructure module) { }
                protected override void RegisterServices(IServiceCollection module) { }
            }
            """;

        // Act
        IReadOnlyCollection<Diagnostic> result = CompilationHelpers.CompileWithTychoGenerator(genericApp);

        // Assert
        static bool IsGenericModuleCausedError(Diagnostic diagnostic)
        {
            return diagnostic.GetMessage().Contains("'TestModule' does not implement inherited abstract member");
        }

        var compilationErrors = result.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        Assert.NotEmpty(compilationErrors);

        var genericModuleErrors = compilationErrors.Where(IsGenericModuleCausedError).ToList();
        Assert.NotEmpty(genericModuleErrors);

        var otherErrors = compilationErrors.Where(d => !IsGenericModuleCausedError(d)).ToList();
        Assert.Empty(otherErrors);
    }
}
