using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.ConfiguringLogging.SUT.Handlers;
using Tycho.IntegrationTests.ConfiguringLogging.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ConfiguringLogging.SUT;

// Handles
public record LogAppRequest : IRequest;

[TychoDefinition]
public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Expects<LogAppRequest>()
           .HandlesWith<LogAppRequestHandler>();

        app.Expects<LogAlphaRequest>()
           .ForwardsTo<AlphaModule>();

        app.Expects<LogBetaRequest>()
           .ForwardsTo<AlphaModule>();
    }

    protected override void DefineEvents(IAppEvents app) { }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<AlphaModule>();
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddTransient<LogAppRequestHandler>();
    }
}
