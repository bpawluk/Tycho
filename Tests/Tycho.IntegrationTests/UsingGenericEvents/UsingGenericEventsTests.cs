using Microsoft.CodeAnalysis;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.UsingGenericEvents;

public sealed class UsingGenericEventsTests
{
    [Fact]
    public void TychoEnables_GenericAppDefinitions()
    {
        // Arrange
        string genericApp =
            """
            using System.Threading;
            using System.Threading.Tasks;
            using Microsoft.Extensions.DependencyInjection;
            using Tycho.Apps;
            using Tycho.Events;

            namespace Tycho.IntegrationTests.UsingGenericEvents.SUT;

            public sealed class TestEvent<T> : IEvent { }

            public sealed class TestEventHandler : IEventHandler<TestEvent<int>>
            {
                public Task HandleAsync(EventContext<TestEvent<int>> context, CancellationToken cancellationToken)
                {
                    return Task.CompletedTask;
                }
            }

            [TychoDefinition]
            public partial class TestApp : TychoApp
            {
                protected override void DefineContract(IAppContract app) { }

                protected override void DefineEvents(IAppEvents app)
                {
                    app.Handles<TestEvent<int>, TestEventHandler>();
                }

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
