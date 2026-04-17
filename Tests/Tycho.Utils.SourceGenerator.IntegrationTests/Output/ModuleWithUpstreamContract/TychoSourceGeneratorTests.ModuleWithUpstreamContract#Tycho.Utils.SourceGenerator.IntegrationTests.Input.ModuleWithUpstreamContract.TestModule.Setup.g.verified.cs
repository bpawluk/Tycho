//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithUpstreamContract.TestModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithUpstreamContract
{
    public partial class TestModule : TychoModule
    {
        protected override void __AutoSetup__(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddTransient<IPublisher, TestModulePublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IParent, TestModuleParent>(module);
        }
    }
}
