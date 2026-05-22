using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;

namespace Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Modules;

[TychoDefinition]
public partial class HelperExtensionModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }
    protected override void DefineEvents(IModuleEvents module) { }
    protected override void IncludeModules(IModuleStructure module) { }
    protected override void RegisterServices(IServiceCollection module) { }
}
