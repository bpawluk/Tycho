//HintName: AppOuter.AppInner.TestApp.Setup.g.cs
public partial class AppOuter
{
    public partial class AppInner
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
