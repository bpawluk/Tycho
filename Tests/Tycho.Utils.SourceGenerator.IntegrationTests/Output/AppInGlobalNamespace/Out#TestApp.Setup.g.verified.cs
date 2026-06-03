//HintName: TestApp.Setup.g.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tycho.Apps;
using Tycho.Events.Serialization;
using Tycho.Modules.Instance;

public class TestAppSetup
{
    public static void Setup(IServiceCollection app)
    {
        ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestAppEventSerializer>(app);
        ServiceCollectionServiceExtensions.AddTransient<ITestAppPublisher, TestAppPublisher>(app);
    }
}
