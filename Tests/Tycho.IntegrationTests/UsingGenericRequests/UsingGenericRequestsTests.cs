using Microsoft.CodeAnalysis;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.UsingGenericRequests;

public sealed class UsingGenericRequestsTests
{
    [Fact]
    public void TychoEnables_GenericRequestDefinitions()
    {
        // Arrange
        string genericApp =
            """
            using System.Threading;
            using System.Threading.Tasks;
            using Microsoft.Extensions.DependencyInjection;
            using Tycho.Apps;
            using Tycho.Requests;

            namespace Tycho.IntegrationTests.UsingGenericRequests.SUT;

            public sealed class TestRequest<T> : IRequest { }

            public sealed class TestRequestHandler : IRequestHandler<TestRequest<int>>
            {
                public Task HandleAsync(TestRequest<int> requestData, CancellationToken cancellationToken)
                {
                    return Task.CompletedTask;
                }
            }

            [TychoDefinition]
            public partial class TestApp : TychoApp
            {
                protected override void DefineContract(IAppContract app)
                {
                    app.Handles<TestRequest<int>, TestRequestHandler>();
                }

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
    public void TychoEnables_GenericResponseDefinitions()
    {
        // Arrange
        string genericApp =
            """
            using System.Threading;
            using System.Threading.Tasks;
            using Microsoft.Extensions.DependencyInjection;
            using Tycho.Apps;
            using Tycho.Requests;

            namespace Tycho.IntegrationTests.UsingGenericRequests.SUT;

            public sealed class TestResponse<T> { }

            public sealed class TestRequest : IRequest<TestResponse<int>> { }

            public sealed class TestRequestHandler : IRequestHandler<TestRequest, TestResponse<int>>
            {
                public Task<TestResponse<int>> HandleAsync(TestRequest requestData, CancellationToken cancellationToken)
                {
                    return Task.FromResult(new TestResponse<int>());
                }
            }

            [TychoDefinition]
            public partial class TestApp : TychoApp
            {
                protected override void DefineContract(IAppContract app)
                {
                    app.Handles<TestRequest, TestResponse<int>, TestRequestHandler>();
                }

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
}
