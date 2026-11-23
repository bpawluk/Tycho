using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithoutAttribute;

public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app) { }
    protected override void DefineEvents(IAppEvents app) { }
    protected override void IncludeModules(IAppStructure app) { }
    protected override void RegisterServices(IServiceCollection app) { }
}
