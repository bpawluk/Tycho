//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules.LocalStaticHelperModule.Setup.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules
{
    public class LocalStaticHelperModuleSetup
    {
        public static void Setup(global::Microsoft.Extensions.DependencyInjection.IServiceCollection module)
        {
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton<global::Tycho.Events.Serialization.IEventSerializer, LocalStaticHelperModuleEventSerializer>(module);
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<ILocalStaticHelperModulePublisher, LocalStaticHelperModulePublisher>(module);
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<ILocalStaticHelperModuleParent, LocalStaticHelperModuleParent>(module);
        }
    }
}
