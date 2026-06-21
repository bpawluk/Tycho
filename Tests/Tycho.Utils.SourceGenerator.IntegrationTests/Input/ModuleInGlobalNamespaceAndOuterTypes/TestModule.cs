using Tycho;
using Tycho.Modules;
using Microsoft.Extensions.DependencyInjection;

public partial class Outer
{
    public partial class Inner
    {

        [TychoDefinition]
        public class TestModule : TychoModule
        {
            protected override void DefineContract(IModuleContract module) { }
            protected override void DefineEvents(IModuleEvents module) { }
            protected override void IncludeModules(IModuleStructure module) { }
            protected override void RegisterServices(IServiceCollection module) { }
        }
    }
}
