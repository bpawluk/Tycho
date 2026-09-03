//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules.LocalHelperModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules
{
    public class LocalHelperModuleSetup
    {
        public static void Setup(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, LocalHelperModuleEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<ILocalHelperModulePublisher, LocalHelperModulePublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<ILocalHelperModuleParent, LocalHelperModuleParent>(module);
        }
    }
}
