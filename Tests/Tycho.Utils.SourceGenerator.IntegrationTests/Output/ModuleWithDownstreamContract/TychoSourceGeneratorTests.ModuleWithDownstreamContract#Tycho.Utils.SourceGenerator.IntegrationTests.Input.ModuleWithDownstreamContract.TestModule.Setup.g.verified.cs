//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract.TestModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract
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
