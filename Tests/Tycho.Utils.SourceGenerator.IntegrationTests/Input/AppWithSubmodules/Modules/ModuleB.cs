using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.Modules;

public class ModuleB : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }
    protected override void DefineEvents(IModuleEvents module) { }
    protected override void IncludeModules(IModuleStructure module) { }
    protected override void RegisterServices(IServiceCollection module) { }
}
