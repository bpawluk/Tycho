using Tycho;
using Tycho.Apps;
using Microsoft.Extensions.DependencyInjection;

[TychoDefinition]
public partial class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app) { }
    protected override void DefineEvents(IAppEvents app) { }
    protected override void IncludeModules(IAppStructure app) { }
    protected override void RegisterServices(IServiceCollection app) { }
}
