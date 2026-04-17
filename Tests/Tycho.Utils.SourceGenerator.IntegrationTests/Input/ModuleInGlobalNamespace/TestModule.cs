using Tycho;
using Tycho.Modules;
using Microsoft.Extensions.DependencyInjection;

[TychoDefinition]
public partial class TestModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }
    protected override void DefineEvents(IModuleEvents module) { }
    protected override void IncludeModules(IModuleStructure module) { }
    protected override void RegisterServices(IServiceCollection module) { }
}
