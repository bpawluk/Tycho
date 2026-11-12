//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithSubmodules.TestModule.Setup.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithSubmodules
{
    public class TestModuleSetup
    {
        public static void Setup(global::Microsoft.Extensions.DependencyInjection.IServiceCollection module)
        {
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton<global::Tycho.Events.Serialization.IEventSerializer, TestModuleEventSerializer>(module);
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithSubmodules.Modules.Outer<global::System.Int32>.Inner.IModuleA, global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithSubmodules.Modules.Outer<global::System.Int32>.Inner.ModuleAFacade>(module);
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithSubmodules.Modules.IModuleB, global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithSubmodules.Modules.ModuleBFacade>(module);
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<ITestModulePublisher, TestModulePublisher>(module);
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<ITestModuleParent, TestModuleParent>(module);
        }
    }
}
