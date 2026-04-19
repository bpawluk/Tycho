//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithSubmodules.Modules.ModuleB.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithSubmodules.Modules
{
    public partial class ModuleB : TychoModule
    {
        protected override void __AutoSetup__(IServiceCollection module)
        {
            ServiceCollectionServiceExtensions.AddTransient<IPublisher, ModuleBPublisher>(module);
            ServiceCollectionServiceExtensions.AddTransient<IParent, ModuleBParent>(module);
        }
    }
}
