//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInNamespaceAndOuterTypes.Outer.Inner.TestApp.Setup.g.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tycho.Apps;
using Tycho.Events.Serialization;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInNamespaceAndOuterTypes
{
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
}
