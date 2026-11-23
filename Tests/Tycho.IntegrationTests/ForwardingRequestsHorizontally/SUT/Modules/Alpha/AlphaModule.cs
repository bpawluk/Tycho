using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Alpha.Handlers;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Alpha;

// Handles
public record AlphaRequest(TestResult Result) : IRequest;
public record AlphaRequestWithResponse(TestResult Result) : IRequest<string>;

[TychoDefinition]
public class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Requires<AlphaRequest>();
        module.Requires<AlphaRequestWithResponse, string>();

        module.Requires<Request>();
        module.Requires<RequestWithResponse, string>();

        module.Expects<AlphaRequest>()
              .HandlesWith<AlphaRequestHandler>();

        module.Expects<AlphaRequestWithResponse, string>()
              .HandlesWith<AlphaRequestHandler>();

        module.Expects<Request>()
              .HandlesWith<RequestHandler>();

        module.Expects<RequestWithResponse, string>()
              .HandlesWith<RequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
