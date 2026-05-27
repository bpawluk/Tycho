using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInNamespaceAndOuterTypes
{
    public partial class Outer
    {
        public partial class Inner
        {
            [TychoDefinition]
            public partial class TestModule : TychoModule
            {
                protected override void DefineContract(IModuleContract module) { }
                protected override void DefineEvents(IModuleEvents module) { }
                protected override void IncludeModules(IModuleStructure module) { }
                protected override void RegisterServices(IServiceCollection module) { }
            }
        }
    }
}
