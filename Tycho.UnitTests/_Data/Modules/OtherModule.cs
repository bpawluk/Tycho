using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;

namespace Tycho.UnitTests._Data.Modules;

public class OtherModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
