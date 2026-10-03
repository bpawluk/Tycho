using AlphaRequest = Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames.Alpha.Request;
using BetaRequest = Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames.Beta.Request;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames.Handlers;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames;

[TychoDefinition]
public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Expects<AlphaRequest>().HandlesWith<AlphaRequestHandler>();
        app.Expects<BetaRequest>().HandlesWith<BetaRequestHandler>();
    }

    protected override void DefineEvents(IAppEvents app) { }

    protected override void IncludeModules(IAppStructure app) { }

    protected override void RegisterServices(IServiceCollection app) { }
}
