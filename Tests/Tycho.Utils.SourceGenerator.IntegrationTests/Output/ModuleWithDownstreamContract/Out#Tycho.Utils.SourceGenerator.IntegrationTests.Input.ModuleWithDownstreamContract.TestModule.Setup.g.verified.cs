//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract.TestModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract
{
    public class TestModuleSetup
    {
        public static void Setup(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestModuleEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<ITestModulePublisher, TestModulePublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<ITestModuleParent, TestModuleParent>(module);
        }
    }
}
