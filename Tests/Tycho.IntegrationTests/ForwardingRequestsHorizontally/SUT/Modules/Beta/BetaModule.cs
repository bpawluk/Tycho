using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Beta.Handlers;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Beta;

// Handles
public record BetaRequest(TestResult Result) : IRequest;
public record BetaRequestWithResponse(TestResult Result) : IRequest<string>;

internal class BetaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<Request, RequestHandler>()
              .Handles<RequestWithResponse, string, RequestHandler>();

        module.Requires<Request>()
              .Requires<RequestWithResponse, string>();

        module.Handles<BetaRequest, BetaRequestHandler>()
              .Handles<BetaRequestWithResponse, string, BetaRequestHandler>();

        module.Requires<BetaRequest>()
              .Requires<BetaRequestWithResponse, string>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}