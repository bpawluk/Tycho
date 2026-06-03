//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInGenericOuterTypes.Outer`1.Inner`1.TestModule`1.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            public partial class TestModule<TModule> : TychoModule
                where TModule : notnull
            {
                protected override void __AutoSetup__(IServiceCollection module)
                {
                    ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestModuleEventSerializer<TModule>>(module);
                    ServiceCollectionServiceExtensions.AddTransient<ITestModulePublisher<TModule>, TestModulePublisher<TModule>>(module);
                    ServiceCollectionServiceExtensions.AddTransient<IParent, TestModuleParent<TModule>>(module);
                }
            }
        }
    }
}
