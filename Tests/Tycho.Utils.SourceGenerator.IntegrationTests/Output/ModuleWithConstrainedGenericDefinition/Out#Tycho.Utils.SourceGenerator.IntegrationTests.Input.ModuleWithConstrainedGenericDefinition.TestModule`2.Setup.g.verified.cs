//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition.TestModule`2.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;
using Tycho.Modules;
using Tycho.Modules.Instance;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.SharedConstraints;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition
{
    public partial class TestModule<TPayload, TKey> : TychoModule
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
        protected override void __AutoSetup__(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestModuleEventSerializer<TPayload, TKey>>(module);
            ServiceCollectionServiceExtensions.AddTransient<IPublisher, TestModulePublisher<TPayload, TKey>>(module);
            ServiceCollectionServiceExtensions.AddTransient<IParent, TestModuleParent<TPayload, TKey>>(module);
        }
    }
}
