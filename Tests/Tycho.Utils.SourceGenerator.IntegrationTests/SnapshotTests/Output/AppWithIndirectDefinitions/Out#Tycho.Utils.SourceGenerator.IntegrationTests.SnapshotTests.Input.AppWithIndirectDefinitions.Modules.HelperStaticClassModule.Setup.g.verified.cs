//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules.HelperStaticClassModule.Setup.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules
{
    public class HelperStaticClassModuleSetup
    {
        public static void Setup(global::Microsoft.Extensions.DependencyInjection.IServiceCollection module)
        {
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton<global::Tycho.Events.Serialization.IEventSerializer, HelperStaticClassModuleEventSerializer>(module);
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<IHelperStaticClassModulePublisher, HelperStaticClassModulePublisher>(module);
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<IHelperStaticClassModuleParent, HelperStaticClassModuleParent>(module);
        }
    }
}
