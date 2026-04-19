//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents.Modules.ModuleA.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents.Modules
{
    public partial class ModuleA : TychoModule
    {
        protected override void __AutoSetup__(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddTransient<IPublisher, ModuleAPublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IParent, ModuleAParent>(module);
        }
    }
}
