//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInGenericOuterTypes.Outer`1.Inner`1.TestModule`1.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;

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
                public static void Setup(IServiceCollection module)
                {
                    ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestModuleEventSerializer<TModule>>(module);
                    ServiceCollectionServiceExtensions.AddTransient<ITestModulePublisher<TModule>, TestModulePublisher<TModule>>(module);
                    ServiceCollectionServiceExtensions.AddTransient<ITestModuleParent<TModule>, TestModuleParent<TModule>>(module);
                }
            }
        }
    }
}
