//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.Modules.ModuleB.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.Modules
{
    public partial class ModuleB : TychoModule
    {
        protected override void __AutoSetup__(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, ModuleBEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<IModuleBPublisher, ModuleBPublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IParent, ModuleBParent>(module);
        }
    }
}
