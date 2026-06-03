//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithGenericDefinition.TestModule`1.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithGenericDefinition
{
    public partial class TestModule<T> : TychoModule
    {
        protected override void __AutoSetup__(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestModuleEventSerializer<T>>(module);
            ServiceCollectionServiceExtensions.AddTransient<ITestModulePublisher<T>, TestModulePublisher<T>>(module);
            ServiceCollectionServiceExtensions.AddTransient<IParent, TestModuleParent<T>>(module);
        }
    }
}
