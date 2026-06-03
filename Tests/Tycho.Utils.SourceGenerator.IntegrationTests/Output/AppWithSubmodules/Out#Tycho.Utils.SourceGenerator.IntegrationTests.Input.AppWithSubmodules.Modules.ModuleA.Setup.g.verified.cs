//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.Modules.ModuleA.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.Modules
{
    public partial class ModuleA : TychoModule
    {
        protected override void __AutoSetup__(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, ModuleAEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<IModuleA.IPublisher, ModuleAPublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IParent, ModuleAParent>(module);
        }
    }
}
