using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Alpha.Handlers;
using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Beta;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Alpha;

// Handles
public record AlphaInRequest(TestResult Result) : IRequest;
public record AlphaInRequestWithResponse(TestResult Result) : IRequest<string>;

// Requires
public record AlphaOutRequest(TestResult Result) : IRequest;
public record AlphaOutRequestWithResponse(TestResult Result) : IRequest<string>;

internal class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<AlphaInRequest, AlphaInRequestHandler>()
              .Handles<AlphaInRequestWithResponse, string, AlphaInRequestHandler>();

        module.Requires<AlphaOutRequest>()
              .Requires<AlphaOutRequestWithResponse, string>();
    }

    protected override void IncludeModules(IModuleStructure module) 
    {
        module.Uses<BetaModule>(contract =>
        {
            contract.Handle<BetaOutRequest, GammaOutRequestHandler>()
                    .Handle<BetaOutRequestWithResponse, string, GammaOutRequestHandler>();
        });
    }

    protected override void MapEvents(IModuleEvents module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
