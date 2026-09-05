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
        app.Uses<AlphaModule>(app =>
        {
            app.Fulfills<Request>()
               .ForwardsTo<BetaModule>();

            app.Fulfills<RequestWithResponse, string>()
               .ForwardsTo<BetaModule>();

            app.Fulfills<AlphaRequest>()
               .MapsTo<BetaRequest>(request => new(request.Result))
               .ForwardsTo<BetaModule>();

            app.Fulfills<AlphaRequestWithResponse, string>()
               .MapsTo<BetaRequestWithResponse, string>(
                   request => new(request.Result),
                   response => response)
               .ForwardsTo<BetaModule>();
        });

        app.Uses<BetaModule>(app =>
        {
            app.Fulfills<Request>()
               .ForwardsTo<GammaModule>();

            app.Fulfills<RequestWithResponse, string>()
               .ForwardsTo<GammaModule>();

            app.Fulfills<BetaRequest>()
               .MapsTo<GammaRequest>(request => new(request.Result))
               .ForwardsTo<GammaModule>();

            app.Fulfills<BetaRequestWithResponse, string>()
               .MapsTo<GammaRequestWithResponse, string>(
                   request => new(request.Result),
                   response => response)
               .ForwardsTo<GammaModule>();
        });

        app.Uses<GammaModule>(app =>
        {
            app.Fulfills<Request>()
               .HandlesWith<RequestHandler>();

            app.Fulfills<RequestWithResponse, string>()
               .HandlesWith<RequestHandler>();

            app.Fulfills<GammaRequest>()
               .HandlesWith<GammaRequestHandler>();

            app.Fulfills<GammaRequestWithResponse, string>()
               .HandlesWith<GammaRequestHandler>();
        });
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(_testWorkflow);
    }
}
