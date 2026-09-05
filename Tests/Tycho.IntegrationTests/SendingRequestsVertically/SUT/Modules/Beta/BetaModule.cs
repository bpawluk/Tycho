using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Beta.Handlers;
using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Gamma;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Beta;

// Handles
public record BetaInRequest(TestResult Result) : IRequest;
public record BetaInRequestWithResponse(TestResult Result) : IRequest<string>;

// Requires
public record BetaOutRequest(TestResult Result) : IRequest;
public record BetaOutRequestWithResponse(TestResult Result) : IRequest<string>;

[TychoDefinition]
public class BetaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Requires<BetaOutRequest>();
        module.Requires<BetaOutRequestWithResponse, string>();

        module.Expects<BetaInRequest>()
              .HandlesWith<AlphaInRequestHandler>();

        module.Expects<BetaInRequestWithResponse, string>()
              .HandlesWith<AlphaInRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module)
    {
        module.Uses<GammaModule>(module =>
        {
            module.Fulfills<GammaOutRequest>()
                  .HandlesWith<GammaOutRequestHandler>();

            module.Fulfills<GammaOutRequestWithResponse, string>()
                  .HandlesWith<GammaOutRequestHandler>();
        });
    }

    protected override void RegisterServices(IServiceCollection module) { }
}
