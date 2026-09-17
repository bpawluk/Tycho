//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespaceAndOuterTypes.Outer.Inner.TestApp.Setup.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespaceAndOuterTypes
{
    public partial class Outer
    {
        public partial class Inner
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
}
