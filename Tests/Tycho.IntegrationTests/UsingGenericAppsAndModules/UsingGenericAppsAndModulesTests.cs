using Microsoft.CodeAnalysis;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.UsingGenericAppsAndModules;

public sealed class UsingGenericAppsAndModulesTests
{
    [Fact]
    public void TychoEnables_GenericAppDefinitions()
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
        var compilationErrors = result.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        Assert.Empty(compilationErrors);
    }

    [Fact]
    public void TychoEnables_GenericModuleDefinitions()
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
        var compilationErrors = result.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        Assert.Empty(compilationErrors);
    }

    [Fact]
    public void TychoEnables_GenericAppDefinitionsWithConstraints()
    {
        // Arrange
        string constrainedGenericApp =
            """
            using System.Threading;
            using System.Threading.Tasks;
            using Microsoft.Extensions.DependencyInjection;
            using Tycho.Apps;
            using Tycho.Requests;

            namespace Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT;

            public abstract class PayloadBase { }

            public interface IMarker { }

            public sealed class TestRequest<TPayload, TKey> : IRequest
                where TPayload : PayloadBase, IMarker, new()
                where TKey : notnull
            { }

            public sealed class TestRequestHandler<TPayload, TKey> : IRequestHandler<TestRequest<TPayload, TKey>>
                where TPayload : PayloadBase, IMarker, new()
                where TKey : notnull
            {
                public Task HandleAsync(TestRequest<TPayload, TKey> requestData, CancellationToken cancellationToken)
                {
                    return Task.CompletedTask;
                }
            }

            [TychoDefinition]
            public partial class TestApp<TPayload, TKey> : TychoApp
                where TPayload : PayloadBase, IMarker, new()
                where TKey : notnull
            {
                protected override void DefineContract(IAppContract app)
                {
                    app.Handles<TestRequest<TPayload, TKey>, TestRequestHandler<TPayload, TKey>>();
                }

                protected override void DefineEvents(IAppEvents app) { }
                protected override void IncludeModules(IAppStructure app) { }
                protected override void RegisterServices(IServiceCollection app) { }
            }
            """;

        // Act
        IReadOnlyCollection<Diagnostic> result = CompilationHelpers.CompileWithTychoGenerator(constrainedGenericApp);

        // Assert
        var compilationErrors = result.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        Assert.Empty(compilationErrors);
    }

    [Fact]
    public void TychoEnables_GenericModuleDefinitionsWithConstraints()
    {
        // Arrange
        string constrainedGenericModule =
            """
            using System.Threading;
            using System.Threading.Tasks;
            using Microsoft.Extensions.DependencyInjection;
            using Tycho.Modules;
            using Tycho.Requests;

            namespace Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT;

            public abstract class PayloadBase { }

            public interface IMarker { }

            public sealed class TestRequest<TPayload, TKey> : IRequest
                where TPayload : PayloadBase, IMarker, new()
                where TKey : notnull
            { }

            public sealed class TestRequestHandler<TPayload, TKey> : IRequestHandler<TestRequest<TPayload, TKey>>
                where TPayload : PayloadBase, IMarker, new()
                where TKey : notnull
            {
                public Task HandleAsync(TestRequest<TPayload, TKey> requestData, CancellationToken cancellationToken)
                {
                    return Task.CompletedTask;
                }
            }

            [TychoDefinition]
            public partial class TestModule<TPayload, TKey> : TychoModule
                where TPayload : PayloadBase, IMarker, new()
                where TKey : notnull
            {
                protected override void DefineContract(IModuleContract module)
                {
                    module.Handles<TestRequest<TPayload, TKey>, TestRequestHandler<TPayload, TKey>>();
                }

                protected override void DefineEvents(IModuleEvents module) { }
                protected override void IncludeModules(IModuleStructure module) { }
                protected override void RegisterServices(IServiceCollection module) { }
            }
            """;

        // Act
        IReadOnlyCollection<Diagnostic> result = CompilationHelpers.CompileWithTychoGenerator(constrainedGenericModule);

        // Assert
        var compilationErrors = result.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        Assert.Empty(compilationErrors);
    }
}
