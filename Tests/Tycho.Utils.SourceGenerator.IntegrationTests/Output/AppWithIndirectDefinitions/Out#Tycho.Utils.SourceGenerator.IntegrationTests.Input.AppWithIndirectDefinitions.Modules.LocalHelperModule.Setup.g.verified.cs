//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.LocalHelperModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    public partial class LocalHelperModule : TychoModule
    {
        protected override void __AutoSetup__(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, LocalHelperModuleEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<IPublisher, LocalHelperModulePublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IParent, LocalHelperModuleParent>(module);
        }
    }
}
