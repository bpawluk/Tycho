//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules.Modules.ModuleB.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules.Modules
{
    public class ModuleBSetup
    {
        public static void Setup(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, ModuleBEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<IModuleBPublisher, ModuleBPublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IModuleBParent, ModuleBParent>(module);
        }
    }
}
