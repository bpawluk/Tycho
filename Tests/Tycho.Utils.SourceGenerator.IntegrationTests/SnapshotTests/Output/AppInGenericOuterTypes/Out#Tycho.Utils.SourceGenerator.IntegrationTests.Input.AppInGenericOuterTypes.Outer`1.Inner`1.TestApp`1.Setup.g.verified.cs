//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes.Outer`1.Inner`1.TestApp`1.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            public class TestAppSetup<TApp>
                where TApp : new()
            {
                public static void Setup(IServiceCollection app)
                {
                    ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestAppEventSerializer<TApp>>(app);
                    ServiceCollectionServiceExtensions.AddTransient<ITestAppPublisher<TApp>, TestAppPublisher<TApp>>(app);
                }
            }
        }
    }
}
