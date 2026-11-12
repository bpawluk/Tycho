//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithGenericDefinition.TestModule`1.Setup.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithGenericDefinition
{
    public class TestModuleSetup<T>
    {
        public static void Setup(global::Microsoft.Extensions.DependencyInjection.IServiceCollection module)
        {
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton<global::Tycho.Events.Serialization.IEventSerializer, TestModuleEventSerializer<T>>(module);
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<ITestModulePublisher<T>, TestModulePublisher<T>>(module);
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<ITestModuleParent<T>, TestModuleParent<T>>(module);
        }
    }
}
