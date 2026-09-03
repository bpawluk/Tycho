//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithGenericDefinition.TestModule`1.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithGenericDefinition
{
    public class TestModuleSetup<T>
    {
        public static void Setup(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestModuleEventSerializer<T>>(module);
            ServiceCollectionServiceExtensions.AddTransient<ITestModulePublisher<T>, TestModulePublisher<T>>(module);
            ServiceCollectionServiceExtensions.AddTransient<ITestModuleParent<T>, TestModuleParent<T>>(module);
        }
    }
}
