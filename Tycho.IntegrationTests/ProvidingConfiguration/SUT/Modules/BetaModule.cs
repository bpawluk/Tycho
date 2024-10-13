using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ProvidingConfiguration.SUT.Handlers;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ProvidingConfiguration.SUT.Modules;

// Handles
public record GetBetaValueRequest : IRequest<string>;

internal class BetaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<GetBetaValueRequest, string, GetValueRequestHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
