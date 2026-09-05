using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Modules;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Settings;

namespace Tycho.IntegrationTests.ProvidingSettings.SUT;

[TychoDefinition]
public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Expects<GetAlphaValueRequest, string>()
           .ForwardsTo<AlphaModule>();

        app.Expects<GetBetaValueRequest, string>()
           .ForwardsTo<AlphaModule>();

        app.Expects<GetGammaValueRequest, string>()
           .ForwardsTo<GammaModule>();
    }

    protected override void DefineEvents(IAppEvents app) { }

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

    protected override void RegisterServices(IServiceCollection app) { }
}
