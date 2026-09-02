//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithSubmodules.TestModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho.Events.Serialization;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithSubmodules.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithSubmodules
{
    public class TestModuleSetup
    {
        public static void Setup(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestModuleEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<ITestModulePublisher, TestModulePublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<ITestModuleParent, TestModuleParent>(module);
            ServiceCollectionServiceExtensions.AddTransient<Outer<Int32>.Inner.IModuleA, Outer<Int32>.Inner.ModuleAFacade>(module);
            ServiceCollectionServiceExtensions.AddTransient<IModuleB, ModuleBFacade>(module);
        }
    }
}
