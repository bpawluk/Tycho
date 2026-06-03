//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.LocalStaticHelperModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    public partial class LocalStaticHelperModule : TychoModule
    {
        protected override void __AutoSetup__(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, LocalStaticHelperModuleEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<ILocalStaticHelperModulePublisher, LocalStaticHelperModulePublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IParent, LocalStaticHelperModuleParent>(module);
        }
    }
}
