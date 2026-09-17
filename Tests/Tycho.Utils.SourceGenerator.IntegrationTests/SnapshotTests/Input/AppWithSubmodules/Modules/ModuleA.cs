using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules.Modules;

public partial class Outer<TOuter>
{
    public partial class Inner
    {
        [TychoDefinition]
        public class ModuleA : TychoModule
        {
            protected override void DefineContract(IModuleContract module) { }
            protected override void DefineEvents(IModuleEvents module) { }
            protected override void IncludeModules(IModuleStructure module) { }
            protected override void RegisterServices(IServiceCollection module) { }
        }
    }
}
