using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Gamma.Handlers;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Gamma;

// Handles
public record GammaInRequest(TestResult Result) : IRequest;
public record GammaInRequestWithResponse(TestResult Result) : IRequest<string>;

// Requires
public record GammaOutRequest(TestResult Result) : IRequest;
public record GammaOutRequestWithResponse(TestResult Result) : IRequest<string>;

[TychoDefinition]
public class GammaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Requires<GammaOutRequest>();
        module.Requires<GammaOutRequestWithResponse, string>();

        module.Expects<GammaInRequest>()
              .HandlesWith<GammaInRequestHandler>();

        module.Expects<GammaInRequestWithResponse, string>()
              .HandlesWith<GammaInRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
