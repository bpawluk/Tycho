//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.HelperStaticClassModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    public partial class HelperStaticClassModule : TychoModule
    {
        protected override void __AutoSetup__(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, HelperStaticClassModuleEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<IPublisher, HelperStaticClassModulePublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IParent, HelperStaticClassModuleParent>(module);
        }
    }
}
