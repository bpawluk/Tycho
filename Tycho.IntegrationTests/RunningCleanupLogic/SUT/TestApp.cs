using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.RunningCleanupLogic.SUT.Modules;

namespace Tycho.IntegrationTests.RunningCleanupLogic.SUT;

public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app) { }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<AlphaModule>();
        app.Uses<BetaModule>();
    }

    protected override void MapEvents(IAppEvents app) { }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(TestResult.Instance);
    }

    protected override Task Cleanup(IServiceProvider app)
    {
        var result = app.GetRequiredService<TestResult>();
        result.AppCleanupPerformed = true;
        return base.Cleanup(app);
    }
}