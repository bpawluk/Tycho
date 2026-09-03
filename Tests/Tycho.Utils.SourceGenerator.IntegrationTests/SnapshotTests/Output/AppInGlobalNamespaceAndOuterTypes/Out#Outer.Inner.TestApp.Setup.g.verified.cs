//HintName: Outer.Inner.TestApp.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;

public partial class Outer
{
    public partial class Inner
    {
        public class TestAppSetup
        {
            public static void Setup(IServiceCollection app)
            {
                ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestAppEventSerializer>(app);
                ServiceCollectionServiceExtensions.AddTransient<ITestAppPublisher, TestAppPublisher>(app);
            }
        }
    }
}
