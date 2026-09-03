//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithSubmodules.Modules.Outer`1.Inner.ModuleA.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithSubmodules.Modules
{
    public partial class Outer<TOuter>
    {
        public partial class Inner
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
    }
}
