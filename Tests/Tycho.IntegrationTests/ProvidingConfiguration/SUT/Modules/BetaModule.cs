using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ProvidingConfiguration.SUT.Handlers;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ProvidingConfiguration.SUT.Modules;

// Handles
public record GetBetaValueRequest : IRequest<string>;

[TychoDefinition]
public class BetaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<GetBetaValueRequest, string>()
              .HandlesWith<GetValueRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddSingleton(serviceProvider =>
            serviceProvider.GetRequiredService<IConfiguration>().GetSection("Beta"));
    }
}
