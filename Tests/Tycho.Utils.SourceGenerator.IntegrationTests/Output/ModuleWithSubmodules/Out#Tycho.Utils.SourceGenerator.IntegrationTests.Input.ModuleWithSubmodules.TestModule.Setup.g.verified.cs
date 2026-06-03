//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithSubmodules.TestModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;
using Tycho.Modules;
using Tycho.Modules.Instance;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithSubmodules.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithSubmodules
{
    public partial class TestModule : TychoModule
    {
        protected override void __AutoSetup__(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestModuleEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<ITestModulePublisher, TestModulePublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IParent, TestModuleParent>(module);
            ServiceCollectionServiceExtensions.AddTransient<IModuleA, ModuleAFacade>(module);
            ServiceCollectionServiceExtensions.AddTransient<IModuleB, ModuleBFacade>(module);
        }
    }
}
