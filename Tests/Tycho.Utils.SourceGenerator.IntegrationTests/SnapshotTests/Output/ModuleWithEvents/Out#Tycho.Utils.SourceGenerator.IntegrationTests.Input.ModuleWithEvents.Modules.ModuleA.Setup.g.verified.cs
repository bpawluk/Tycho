//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents.Modules.ModuleA.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents.Modules
{
    public class ModuleASetup
    {
        public static void Setup(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, ModuleAEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<IModuleAPublisher, ModuleAPublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IModuleAParent, ModuleAParent>(module);
        }
    }
}
