//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.LocalStaticHelperModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    public class LocalStaticHelperModuleSetup
    {
        public static void Setup(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, LocalStaticHelperModuleEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<ILocalStaticHelperModulePublisher, LocalStaticHelperModulePublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<ILocalStaticHelperModuleParent, LocalStaticHelperModuleParent>(module);
        }
    }
}
