//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.TestApp.Setup.g.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tycho.Apps;
using Tycho.Events.Serialization;
using Tycho.Modules.Instance;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules
{
    public class TestAppSetup
    {
        public static void Setup(IServiceCollection app)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestAppEventSerializer>(app);
            ServiceCollectionServiceExtensions.AddTransient<ITestAppPublisher, TestAppPublisher>(app);
            ServiceCollectionServiceExtensions.AddTransient<IModuleA, ModuleAFacade>(app);
            ServiceCollectionServiceExtensions.AddTransient<IModuleB, ModuleBFacade>(app);
        }
    }
}
