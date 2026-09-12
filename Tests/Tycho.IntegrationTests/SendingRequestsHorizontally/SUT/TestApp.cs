using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Handlers;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Alpha;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Beta;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Gamma;
using Tycho.IntegrationTests._Utils;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT;

// Handles
public record Request(TestResult Result) : IRequest;
public record RequestWithResponse(TestResult Result) : IRequest<string>;

[TychoDefinition]
public class TestApp(TestWorkflow<TestResult> testWorkflow) : TychoApp
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    protected override void DefineContract(IAppContract app)
    {
        app.Expects<Request>()
           .HandlesWith<RequestHandler>();

        app.Expects<RequestWithResponse, string>()
           .HandlesWith<RequestHandler>();
    }

    protected override void DefineEvents(IAppEvents app) { }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<AlphaModule>(app =>
        {
            app.Fulfills<AlphaOutRequest>()
               .HandlesWith<AlphaOutRequestHandler>();

            app.Fulfills<AlphaOutRequestWithResponse, string>()
               .HandlesWith<AlphaOutRequestHandler>();
        });

        app.Uses<BetaModule>(app =>
        {
            app.Fulfills<BetaOutRequest>()
               .HandlesWith<BetaOutRequestHandler>();

            app.Fulfills<BetaOutRequestWithResponse, string>()
               .HandlesWith<BetaOutRequestHandler>();
        });

        app.Uses<GammaModule>(app =>
        {
            app.Fulfills<GammaOutRequest>()
               .HandlesWith<GammaOutRequestHandler>();

            app.Fulfills<GammaOutRequestWithResponse, string>()
               .HandlesWith<GammaOutRequestHandler>();
        });
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(_testWorkflow);
    }
}
