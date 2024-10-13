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

internal class GammaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<GammaInRequest, GammaInRequestHandler>()
              .Handles<GammaInRequestWithResponse, string, GammaInRequestHandler>();

        module.Requires<GammaOutRequest>()
              .Requires<GammaOutRequestWithResponse, string>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
