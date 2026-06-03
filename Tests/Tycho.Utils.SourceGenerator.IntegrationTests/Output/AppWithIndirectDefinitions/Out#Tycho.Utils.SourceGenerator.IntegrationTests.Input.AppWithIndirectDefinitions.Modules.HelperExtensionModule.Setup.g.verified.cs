//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.HelperExtensionModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    public partial class HelperExtensionModule : TychoModule
    {
        protected override void __AutoSetup__(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, HelperExtensionModuleEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<IHelperExtensionModulePublisher, HelperExtensionModulePublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IParent, HelperExtensionModuleParent>(module);
        }
    }
}
