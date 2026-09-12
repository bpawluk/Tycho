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

[TychoDefinition]
public class BetaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Requires<BetaOutRequest>();
        module.Requires<BetaOutRequestWithResponse, string>();

        module.Expects<BetaInRequest>()
              .HandlesWith<BetaInRequestHandler>();

        module.Expects<BetaInRequestWithResponse, string>()
              .HandlesWith<BetaInRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
