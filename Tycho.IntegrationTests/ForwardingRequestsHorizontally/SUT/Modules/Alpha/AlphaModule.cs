using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Alpha.Handlers;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Alpha;

// Handles
public record AlphaRequest(TestResult Result) : IRequest;
public record AlphaRequestWithResponse(TestResult Result) : IRequest<string>;

internal class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<Request, RequestHandler>()
              .Handles<RequestWithResponse, string, RequestHandler>();

        module.Requires<AlphaRequest>()
              .Requires<AlphaRequestWithResponse, string>();

        module.Handles<AlphaRequest, AlphaRequestHandler>()
              .Handles<AlphaRequestWithResponse, string, AlphaRequestHandler>();

        module.Requires<AlphaRequest>()
              .Requires<AlphaRequestWithResponse, string>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}