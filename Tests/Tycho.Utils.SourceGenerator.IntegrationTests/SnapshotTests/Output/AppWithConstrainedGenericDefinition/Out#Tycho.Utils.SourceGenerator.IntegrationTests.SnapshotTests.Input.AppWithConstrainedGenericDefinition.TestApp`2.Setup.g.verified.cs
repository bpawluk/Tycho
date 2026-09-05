//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.TestApp`2.Setup.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition
{
    public class TestAppSetup<TPayload, TKey>
        where TPayload : global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.Model.PayloadBase, global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.IMarker, new()
        where TKey : notnull
    {
        public static void Setup(global::Microsoft.Extensions.DependencyInjection.IServiceCollection app)
        {
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton<global::Tycho.Events.Serialization.IEventSerializer, TestAppEventSerializer<TPayload, TKey>>(app);
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<ITestAppPublisher<TPayload, TKey>, TestAppPublisher<TPayload, TKey>>(app);
        }
    }
}
