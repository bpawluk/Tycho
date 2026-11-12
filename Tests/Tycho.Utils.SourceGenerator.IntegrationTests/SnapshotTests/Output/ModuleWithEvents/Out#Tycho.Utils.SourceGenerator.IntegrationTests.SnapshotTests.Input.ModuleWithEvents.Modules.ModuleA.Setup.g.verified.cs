//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Modules.ModuleA.Setup.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Modules
{
    public class ModuleASetup
    {
        public static void Setup(global::Microsoft.Extensions.DependencyInjection.IServiceCollection module)
        {
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton<global::Tycho.Events.Serialization.IEventSerializer, ModuleAEventSerializer>(module);
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<IModuleAPublisher, ModuleAPublisher>(module);
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<IModuleAParent, ModuleAParent>(module);
        }
    }
}
