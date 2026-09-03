//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.HelperExtensionModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    public class HelperExtensionModuleSetup
    {
        public static void Setup(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, HelperExtensionModuleEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<IHelperExtensionModulePublisher, HelperExtensionModulePublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IHelperExtensionModuleParent, HelperExtensionModuleParent>(module);
        }
    }
}
