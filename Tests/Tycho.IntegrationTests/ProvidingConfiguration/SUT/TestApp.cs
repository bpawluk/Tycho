using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.ProvidingConfiguration.SUT.Handlers;
using Tycho.IntegrationTests.ProvidingConfiguration.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ProvidingConfiguration.SUT;

// Handles
public record GetAppValueRequest : IRequest<string>;

[TychoDefinition]
public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Expects<GetAppValueRequest, string>()
           .HandlesWith<GetValueRequestHandler>();

        app.Expects<GetAlphaValueRequest, string>()
           .ForwardsTo<AlphaModule>();

        app.Expects<GetBetaValueRequest, string>()
           .ForwardsTo<BetaModule>();
    }

    protected override void DefineEvents(IAppEvents app) { }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<AlphaModule>()
           .Uses<BetaModule>();
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(serviceProvider =>
            serviceProvider.GetRequiredService<IConfiguration>().GetSection("App"));
    }
}
