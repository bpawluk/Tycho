using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;

namespace Tycho.IntegrationTests.RunningCleanupLogic.SUT.Modules;

internal class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddSingleton(TestResult.Instance);
    }

    protected override Task Cleanup(IServiceProvider module)
    {
        var result = module.GetRequiredService<TestResult>();
        result.AlphaModuleCleanupPerformed = true;
        return base.Cleanup(module);
    }
}