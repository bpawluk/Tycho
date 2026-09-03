//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules.HelperClassModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules
{
    public class HelperClassModuleSetup
    {
        public static void Setup(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, HelperClassModuleEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<IHelperClassModulePublisher, HelperClassModulePublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IHelperClassModuleParent, HelperClassModuleParent>(module);
        }
    }
}
