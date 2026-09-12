using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Gamma.Handlers;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Gamma;

// Handles
public record GammaRequest(TestResult Result) : IRequest;
public record GammaRequestWithResponse(TestResult Result) : IRequest<string>;

[TychoDefinition]
public class GammaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Requires<GammaRequest>();
        module.Requires<GammaRequestWithResponse, string>();

        module.Requires<Request>();
        module.Requires<RequestWithResponse, string>();

        module.Expects<GammaRequest>()
              .HandlesWith<GammaRequestHandler>();

        module.Expects<GammaRequestWithResponse, string>()
              .HandlesWith<GammaRequestHandler>();

        module.Expects<Request>()
              .HandlesWith<RequestHandler>();

        module.Expects<RequestWithResponse, string>()
              .HandlesWith<RequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
