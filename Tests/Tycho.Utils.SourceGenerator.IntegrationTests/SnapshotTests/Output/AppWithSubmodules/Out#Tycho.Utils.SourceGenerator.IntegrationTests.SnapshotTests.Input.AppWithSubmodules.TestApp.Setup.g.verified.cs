//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules.TestApp.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho.Events.Serialization;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules
{
    public class TestAppSetup
    {
        public static void Setup(IServiceCollection app)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestAppEventSerializer>(app);
            ServiceCollectionServiceExtensions.AddTransient<ITestAppPublisher, TestAppPublisher>(app);
            ServiceCollectionServiceExtensions.AddTransient<Outer<String>.Inner.IModuleA, Outer<String>.Inner.ModuleAFacade>(app);
            ServiceCollectionServiceExtensions.AddTransient<IModuleB, ModuleBFacade>(app);
        }
    }
}
