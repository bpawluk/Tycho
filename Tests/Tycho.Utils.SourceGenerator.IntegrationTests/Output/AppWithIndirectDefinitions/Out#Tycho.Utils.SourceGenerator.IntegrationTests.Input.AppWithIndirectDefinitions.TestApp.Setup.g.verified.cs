//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.TestApp.Setup.g.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tycho.Apps;
using Tycho.Events.Serialization;
using Tycho.Modules.Instance;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions
{
    public class TestAppSetup
    {
        public static void Setup(IServiceCollection app)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestAppEventSerializer>(app);
            ServiceCollectionServiceExtensions.AddTransient<ITestAppPublisher, TestAppPublisher>(app);
            ServiceCollectionServiceExtensions.AddTransient<IHelperExtensionModule, HelperExtensionModuleFacade>(app);
            ServiceCollectionServiceExtensions.AddTransient<IHelperStaticClassModule, HelperStaticClassModuleFacade>(app);
            ServiceCollectionServiceExtensions.AddTransient<IHelperClassModule, HelperClassModuleFacade>(app);
            ServiceCollectionServiceExtensions.AddTransient<ILocalStaticHelperModule, LocalStaticHelperModuleFacade>(app);
            ServiceCollectionServiceExtensions.AddTransient<ILocalHelperModule, LocalHelperModuleFacade>(app);
        }
    }
}
