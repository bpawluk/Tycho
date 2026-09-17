using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Beta.Handlers;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Beta;

// Handles
public record BetaRequest(TestResult Result) : IRequest;
public record BetaRequestWithResponse(TestResult Result) : IRequest<string>;

[TychoDefinition]
public class BetaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Requires<BetaRequest>();
        module.Requires<BetaRequestWithResponse, string>();

        module.Requires<Request>();
        module.Requires<RequestWithResponse, string>();

        module.Expects<BetaRequest>()
              .HandlesWith<BetaRequestHandler>();

        module.Expects<BetaRequestWithResponse, string>()
              .HandlesWith<BetaRequestHandler>();

        module.Expects<Request>()
              .HandlesWith<RequestHandler>();

        module.Expects<RequestWithResponse, string>()
              .HandlesWith<RequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
