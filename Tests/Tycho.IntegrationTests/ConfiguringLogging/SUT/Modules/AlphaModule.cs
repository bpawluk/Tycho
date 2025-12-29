using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ConfiguringLogging.SUT.Handlers;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ConfiguringLogging.SUT.Modules;

// Handles
public record LogAlphaRequest : IRequest;

internal class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Forwards<LogBetaRequest, BetaModule>()
              .Handles<LogAlphaRequest, LogAlphaRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module)
    {
        module.Uses<BetaModule>();
    }

    protected override void RegisterServices(IServiceCollection module) { }
}