//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.HelperStaticClassModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    public class HelperStaticClassModuleSetup
    {
        public static void Setup(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, HelperStaticClassModuleEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<IHelperStaticClassModulePublisher, HelperStaticClassModulePublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IHelperStaticClassModuleParent, HelperStaticClassModuleParent>(module);
        }
    }
}
