using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;

namespace Tycho.IntegrationTests.RunningCleanupLogic.SUT.Modules;

internal class BetaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void IncludeModules(IModuleStructure module) 
    {
        module.Uses<GammaModule>();
    }

    protected override void MapEvents(IModuleEvents module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddSingleton(TestResult.Instance);
    }

    protected override Task Cleanup(IServiceProvider module)
    {
        var result = module.GetRequiredService<TestResult>();
        result.BetaModuleCleanupPerformed = true;
        return base.Cleanup(module);
    }
}