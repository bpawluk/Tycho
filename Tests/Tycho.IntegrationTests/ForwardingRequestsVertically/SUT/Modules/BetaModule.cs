using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules;

// Handles
public record BetaRequest(TestResult Result) : IRequest;
public record BetaRequestWithResponse(TestResult Result) : IRequest<string>;

internal class BetaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Forwards<Request, GammaModule>()
              .Forwards<RequestWithResponse, string, GammaModule>();

        module.Requires<Request>()
              .Requires<RequestWithResponse, string>();

        module.ForwardsAs<BetaRequest, GammaRequest, GammaModule>(
                  requestData => new(requestData.Result))
              .ForwardsAs<BetaRequestWithResponse, string, GammaRequestWithResponse, string, GammaModule>(
                  requestData => new(requestData.Result),
                  response => response);

        module.Requires<BetaRequest>()
              .Requires<BetaRequestWithResponse, string>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module)
    {
        module.Uses<GammaModule>(contract =>
        {
            contract.Expose<Request>()
                    .Expose<RequestWithResponse, string>();

            contract.ExposeAs<GammaRequest, BetaRequest>(
                        requestData => new(requestData.Result))
                    .ExposeAs<GammaRequestWithResponse, string, BetaRequestWithResponse, string>(
                        requestData => new(requestData.Result),
                        response => response);
        });
    }

    protected override void RegisterServices(IServiceCollection module) { }
}