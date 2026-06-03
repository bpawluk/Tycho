//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition.TestModule`2.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.SharedConstraints;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition
{
    public class TestModuleSetup<TPayload, TKey>
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
        public static void Setup(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestModuleEventSerializer<TPayload, TKey>>(module);
            ServiceCollectionServiceExtensions.AddTransient<ITestModulePublisher<TPayload, TKey>, TestModulePublisher<TPayload, TKey>>(module);
            ServiceCollectionServiceExtensions.AddTransient<ITestModuleParent<TPayload, TKey>, TestModuleParent<TPayload, TKey>>(module);
        }
    }
}
