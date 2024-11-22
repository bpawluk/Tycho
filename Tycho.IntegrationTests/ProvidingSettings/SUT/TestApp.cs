using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Modules;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Settings;

namespace Tycho.IntegrationTests.ProvidingSettings.SUT;

public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Forwards<GetAlphaValueRequest, string, AlphaModule>()
           .Forwards<GetBetaValueRequest, string, AlphaModule>()
           .Forwards<GetGammaValueRequest, string, GammaModule>();     
    }

    protected override void IncludeModules(IAppStructure app)
    {
        var moduleSettings = new ModuleSettings()
        {
            AlphaValue = "Alpha",
            BetaValue = "Beta"
        };
        app.Uses<AlphaModule>(moduleSettings)
           .Uses<GammaModule>(moduleSettings);
    }

    protected override void MapEvents(IAppEvents app) { }

    protected override void RegisterServices(IServiceCollection app) { }
}