using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ProvidingConfiguration.SUT.Handlers;
using TychoV2.Modules;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.ProvidingConfiguration.SUT.Modules;

// Handles
public record GetAlphaValueRequest : IRequest<string>;

internal class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<GetAlphaValueRequest, string, GetValueRequestHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
