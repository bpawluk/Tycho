//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInGenericOuterTypes.Outer`1.Inner`1.TestModule`1.Setup.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            public class TestModuleSetup<TModule>
                where TModule : notnull
            {
                public static void Setup(global::Microsoft.Extensions.DependencyInjection.IServiceCollection module)
                {
                    global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton<global::Tycho.Events.Serialization.IEventSerializer, TestModuleEventSerializer<TModule>>(module);
                    global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<ITestModulePublisher<TModule>, TestModulePublisher<TModule>>(module);
                    global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<ITestModuleParent<TModule>, TestModuleParent<TModule>>(module);
                }
            }
        }
    }
}
