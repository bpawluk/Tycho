using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ConfiguringLogging.SUT.Handlers;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ConfiguringLogging.SUT.Modules;

// Handles
public record LogBetaRequest : IRequest;

internal class BetaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<LogBetaRequest, LogBetaRequestHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}