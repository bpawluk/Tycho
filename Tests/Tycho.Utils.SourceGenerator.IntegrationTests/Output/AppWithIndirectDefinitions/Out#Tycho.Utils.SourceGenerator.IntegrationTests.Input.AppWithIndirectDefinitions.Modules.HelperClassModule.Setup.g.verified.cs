//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.HelperClassModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    public partial class HelperClassModule : TychoModule
    {
        protected override void __AutoSetup__(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, HelperClassModuleEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<IPublisher, HelperClassModulePublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IParent, HelperClassModuleParent>(module);
        }
    }
}
