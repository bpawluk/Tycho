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

internal class BetaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<BetaInRequest, AlphaInRequestHandler>()
              .Handles<BetaInRequestWithResponse, string, AlphaInRequestHandler>();

        module.Requires<BetaOutRequest>()
              .Requires<BetaOutRequestWithResponse,string>();
    }

    protected override void IncludeModules(IModuleStructure module) 
    {
        module.Uses<GammaModule>(contract =>
        {
            contract.Handle<GammaOutRequest, GammaOutRequestHandler>()
                    .Handle<GammaOutRequestWithResponse, string, GammaOutRequestHandler>();
        });
    }

    protected override void MapEvents(IModuleEvents module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
