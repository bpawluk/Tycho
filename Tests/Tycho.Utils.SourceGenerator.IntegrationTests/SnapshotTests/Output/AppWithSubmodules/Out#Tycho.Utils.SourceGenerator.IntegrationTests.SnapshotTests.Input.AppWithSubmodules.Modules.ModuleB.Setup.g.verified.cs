//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules.Modules.ModuleB.Setup.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules.Modules
{
    public class ModuleBSetup
    {
        public static void Setup(global::Microsoft.Extensions.DependencyInjection.IServiceCollection module)
        {
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton<global::Tycho.Events.Serialization.IEventSerializer, ModuleBEventSerializer>(module);
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<IModuleBPublisher, ModuleBPublisher>(module);
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<IModuleBParent, ModuleBParent>(module);
        }
    }
}
