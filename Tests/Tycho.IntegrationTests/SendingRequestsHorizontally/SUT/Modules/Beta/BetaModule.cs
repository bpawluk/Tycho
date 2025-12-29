using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Beta.Handlers;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Beta;

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
        module.Handles<BetaInRequest, BetaInRequestHandler>()
              .Handles<BetaInRequestWithResponse, string, BetaInRequestHandler>();

        module.Requires<BetaOutRequest>()
              .Requires<BetaOutRequestWithResponse, string>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}