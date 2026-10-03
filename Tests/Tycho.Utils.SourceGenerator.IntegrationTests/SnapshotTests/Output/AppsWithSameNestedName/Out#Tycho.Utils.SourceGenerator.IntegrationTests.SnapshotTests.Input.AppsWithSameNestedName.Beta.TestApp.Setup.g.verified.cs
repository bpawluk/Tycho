//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName.Beta.TestApp.Setup.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName
{
    public partial class Beta
    {
        public class TestAppSetup
        {
            public static void Setup(global::Microsoft.Extensions.DependencyInjection.IServiceCollection app)
            {
                global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton<global::Tycho.Events.Serialization.IEventSerializer, TestAppEventSerializer>(app);
                global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<ITestAppPublisher, TestAppPublisher>(app);
            }
        }
    }
}
