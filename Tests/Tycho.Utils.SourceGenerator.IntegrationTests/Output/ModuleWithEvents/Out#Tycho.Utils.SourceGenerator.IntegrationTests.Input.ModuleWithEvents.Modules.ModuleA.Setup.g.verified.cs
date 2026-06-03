//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents.Modules.ModuleA.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents.Modules
{
    public partial class ModuleA : TychoModule
    {
        protected override void __AutoSetup__(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, ModuleAEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<IModuleAPublisher, ModuleAPublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IModuleAParent, ModuleAParent>(module);
        }
    }
}
