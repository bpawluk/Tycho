//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithGenericDefinition.TestApp`1.Setup.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithGenericDefinition
{
    public class TestAppSetup<T>
    {
        public static void Setup(global::Microsoft.Extensions.DependencyInjection.IServiceCollection app)
        {
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton<global::Tycho.Events.Serialization.IEventSerializer, TestAppEventSerializer<T>>(app);
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<ITestAppPublisher<T>, TestAppPublisher<T>>(app);
        }
    }
}
