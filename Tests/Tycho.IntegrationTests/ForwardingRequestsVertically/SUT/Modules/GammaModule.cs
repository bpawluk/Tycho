using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules.Handlers;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules;

// Handles
public record GammaRequest(TestResult Result) : IRequest;
public record GammaRequestWithResponse(TestResult Result) : IRequest<string>;

internal class GammaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<Request, RequestHandler>()
              .Handles<RequestWithResponse, string, RequestHandler>();

        module.Requires<Request>()
              .Requires<RequestWithResponse, string>();

        module.Handles<GammaRequest, GammaRequestHandler>()
              .Handles<GammaRequestWithResponse, string, GammaRequestHandler>();

        module.Requires<GammaRequest>()
              .Requires<GammaRequestWithResponse, string>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}