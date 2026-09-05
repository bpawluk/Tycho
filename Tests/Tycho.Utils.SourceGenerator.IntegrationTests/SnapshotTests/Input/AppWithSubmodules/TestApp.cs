using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules;

[TychoDefinition]
public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app) { }
    protected override void DefineEvents(IAppEvents app) { }
    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<Outer<string>.Inner.ModuleA>();
        app.Uses<ModuleB>(_ => { });
    }
    protected override void RegisterServices(IServiceCollection app) { }
}
