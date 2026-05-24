using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            [TychoDefinition]
            public partial class TestModule<TModule> : TychoModule
                where TModule : notnull
            {
                protected override void DefineContract(IModuleContract module) { }
                protected override void DefineEvents(IModuleEvents module) { }
                protected override void IncludeModules(IModuleStructure module) { }
                protected override void RegisterServices(IServiceCollection module) { }
            }
        }
    }
}
