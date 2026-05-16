using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;

namespace Tycho.IntegrationTests.RunningCleanupLogic.SUT.Modules;

[TychoDefinition]
public partial class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddSingleton(TestResult.Instance);
    }

    protected override Task Cleanup(IServiceProvider module)
    {
        TestResult result = module.GetRequiredService<TestResult>();
        result.AlphaModuleCleanupPerformed = true;
        return base.Cleanup(module);
    }
}
