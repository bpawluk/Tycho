//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithGenericDefinition.TestApp`1.Setup.g.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tycho.Apps;
using Tycho.Events.Serialization;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithGenericDefinition
{
    public class TestAppSetup<T>
    {
        public static void Setup(IServiceCollection app)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestAppEventSerializer<T>>(app);
            ServiceCollectionServiceExtensions.AddTransient<ITestAppPublisher<T>, TestAppPublisher<T>>(app);
        }
    }
}
