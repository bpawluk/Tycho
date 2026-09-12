using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules;

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
              .MapsTo<GammaRequest>(request => new(request.Result))
              .ForwardsTo<GammaModule>();

        module.Expects<BetaRequestWithResponse, string>()
              .MapsTo<GammaRequestWithResponse, string>(
                  request => new(request.Result),
                  response => response)
              .ForwardsTo<GammaModule>();

        module.Expects<Request>()
              .ForwardsTo<GammaModule>();

        module.Expects<RequestWithResponse, string>()
              .ForwardsTo<GammaModule>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module)
    {
        module.Uses<GammaModule>(module =>
        {
            module.Fulfills<Request>()
                  .Exposes();

            module.Fulfills<RequestWithResponse, string>()
                  .Exposes();

            module.Fulfills<GammaRequest>()
                  .MapsTo<BetaRequest>(request => new(request.Result))
                  .Exposes();

            module.Fulfills<GammaRequestWithResponse, string>()
                  .MapsTo<BetaRequestWithResponse, string>(
                      request => new(request.Result),
                      response => response)
                  .Exposes();
        });
    }

    protected override void RegisterServices(IServiceCollection module) { }
}
