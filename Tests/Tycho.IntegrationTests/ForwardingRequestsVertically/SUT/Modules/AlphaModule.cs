using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules;

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
              .MapsTo<BetaRequest>(request => new(request.Result))
              .ForwardsTo<BetaModule>();

        module.Expects<AlphaRequestWithResponse, string>()
              .MapsTo<BetaRequestWithResponse, string>(
                  request => new(request.Result),
                  response => response)
              .ForwardsTo<BetaModule>();

        module.Expects<Request>()
              .ForwardsTo<BetaModule>();

        module.Expects<RequestWithResponse, string>()
              .ForwardsTo<BetaModule>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module)
    {
        module.Uses<BetaModule>(module =>
        {
            module.Fulfills<Request>()
                  .Exposes();

            module.Fulfills<RequestWithResponse, string>()
                  .Exposes();

            module.Fulfills<BetaRequest>()
                  .MapsTo<AlphaRequest>(request => new(request.Result))
                  .Exposes();

            module.Fulfills<BetaRequestWithResponse, string>()
                  .MapsTo<AlphaRequestWithResponse, string>(
                      request => new(request.Result),
                      response => response)
                  .Exposes();
        });
    }

    protected override void RegisterServices(IServiceCollection module) { }
}
