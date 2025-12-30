using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Modules;


namespace Tycho.SampleConsumer
{
    [AppDefinition]
    public partial class TestApp : TychoApp
    {
        protected override void DefineContract(IAppContract app)
        {
        }

        protected override void DefineEvents(IAppEvents app)
        {
        }

        protected override void IncludeModules(IAppStructure app)
        {
        }

        protected override void RegisterServices(IServiceCollection app)
        {
        }
    }

    [ModuleDefinition]
    public partial class TestModule : TychoModule
    {
        protected override void DefineContract(IModuleContract module)
        {
        }

        protected override void DefineEvents(IModuleEvents module)
        {
        }

        protected override void IncludeModules(IModuleStructure module)
        {
        }

        protected override void RegisterServices(IServiceCollection module)
        {
        }
    }
}
