//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInNamespace.TestModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInNamespace
{
    public partial class TestModule : TychoModule
    {
        protected override void __AutoSetup__(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestModuleEventSerializer>(module);
            ServiceCollectionServiceExtensions.AddTransient<IPublisher, TestModulePublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IParent, TestModuleParent>(module);
        }
    }
}
