using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ProvidingConfiguration.SUT.Modules;
using TychoV2.Apps;

namespace Tycho.IntegrationTests.ProvidingConfiguration.SUT;

public class TestApp(IConfiguration appConfig) : TychoApp
{
    private readonly IConfiguration _appConfig = appConfig;

    protected override void DefineContract(IAppContract app)
    {
        app.Forwards<GetAlphaValueRequest, string, AlphaModule>()
           .Forwards<GetBetaValueRequest, string, BetaModule>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<AlphaModule>(configurationDefinition: moduleConfig =>
        {
            moduleConfig.AddConfiguration(_appConfig.GetSection("Alpha"));
        });

        app.Uses<BetaModule>(configurationDefinition: moduleConfig =>
        {
            moduleConfig.AddConfiguration(_appConfig.GetSection("Beta"));
        });
    }

    protected override void MapEvents(IAppEvents app) { }


    protected override void RegisterServices(IServiceCollection app) { }
}
