using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules;

// Handles
public record AlphaRequest(TestResult Result) : IRequest;
public record AlphaRequestWithResponse(TestResult Result) : IRequest<string>;

internal class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Forwards<Request, BetaModule>()
              .Forwards<RequestWithResponse, string, BetaModule>();

        module.Requires<Request>()
              .Requires<RequestWithResponse, string>();

        module.ForwardsAs<AlphaRequest, BetaRequest, BetaModule>(
                  requestData => new(requestData.Result))
              .ForwardsAs<AlphaRequestWithResponse, string, BetaRequestWithResponse, string, BetaModule>(
                  requestData => new(requestData.Result),
                  response => response);

        module.Requires<AlphaRequest>()
              .Requires<AlphaRequestWithResponse, string>();
    }

    protected override void IncludeModules(IModuleStructure module)
    {
        module.Uses<BetaModule>(contract =>
        {
            contract.Expose<Request>()
                    .Expose<RequestWithResponse, string>();

            contract.ExposeAs<BetaRequest, AlphaRequest>(
                        requestData => new(requestData.Result))
                    .ExposeAs<BetaRequestWithResponse, string, AlphaRequestWithResponse, string>(
                        requestData => new(requestData.Result),
                        response => response);
        });
    }

    protected override void MapEvents(IModuleEvents module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}