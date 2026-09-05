using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ConfiguringLogging.SUT.Handlers;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ConfiguringLogging.SUT.Modules;

// Handles
public record LogAlphaRequest : IRequest;

[TychoDefinition]
public class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<LogAlphaRequest>()
              .HandlesWith<LogAlphaRequestHandler>();

        module.Expects<LogBetaRequest>()
              .ForwardsTo<BetaModule>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module)
    {
        module.Uses<BetaModule>();
    }

    protected override void RegisterServices(IServiceCollection module) { }
}
