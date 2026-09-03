//HintName: AppOuter.AppInner.TestApp.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;

public partial class AppOuter
{
    public partial class AppInner
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
