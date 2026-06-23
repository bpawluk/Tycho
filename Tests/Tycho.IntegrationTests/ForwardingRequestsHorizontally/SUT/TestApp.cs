using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Handlers;
using Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Alpha;
using Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Beta;
using Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Gamma;
using Tycho.IntegrationTests._Utils;
using Tycho.Requests;
using static Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.RequestToMapWithResponse;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT;

// Handles
public record Request(TestResult Result) : IRequest;
public record RequestWithResponse(TestResult Result) : IRequest<string>;
public record RequestToMap(TestResult Result) : IRequest;
public record RequestToMapWithResponse(TestResult Result) : IRequest<Response>
{
    public record Response(string Value);
}

[TychoDefinition]
public class TestApp(TestWorkflow<TestResult> testWorkflow) : TychoApp
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    protected override void DefineContract(IAppContract app)
    {
        app.Expects<Request>()
           .ForwardsTo<AlphaModule>();

        app.Expects<RequestWithResponse, string>()
           .ForwardsTo<AlphaModule>();

        app.Expects<RequestToMap>()
           .MapsTo<AlphaRequest>(request => new(request.Result))
           .ForwardsTo<AlphaModule>();

        app.Expects<RequestToMapWithResponse, Response>()
           .MapsTo<AlphaRequestWithResponse, string>(
               request => new(request.Result),
               response => new(response))
           .ForwardsTo<AlphaModule>();
    }

    protected override void DefineEvents(IAppEvents app) { }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<AlphaModule>(contract =>
        {
            contract.Forward<Request, BetaModule>()
                    .Forward<RequestWithResponse, string, BetaModule>();

            contract.ForwardAs<AlphaRequest, BetaRequest, BetaModule>(
                        requestData => new(requestData.Result))
                    .ForwardAs<AlphaRequestWithResponse, string, BetaRequestWithResponse, string, BetaModule>(
                        requestData => new(requestData.Result),
                        response => response);
        });

        app.Uses<BetaModule>(contract =>
        {
            contract.Forward<Request, GammaModule>()
                    .Forward<RequestWithResponse, string, GammaModule>();

            contract.ForwardAs<BetaRequest, GammaRequest, GammaModule>(
                        requestData => new(requestData.Result))
                    .ForwardAs<BetaRequestWithResponse, string, GammaRequestWithResponse, string, GammaModule>(
                        requestData => new(requestData.Result),
                        response => response);
        });

        app.Uses<GammaModule>(contract =>
        {
            contract.Handle<Request, RequestHandler>()
                    .Handle<RequestWithResponse, string, RequestHandler>();

            contract.Handle<GammaRequest, GammaRequestHandler>()
                    .Handle<GammaRequestWithResponse, string, GammaRequestHandler>();
        });
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(_testWorkflow);
    }
}
