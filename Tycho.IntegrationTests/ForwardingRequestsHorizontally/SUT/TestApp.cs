using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Handlers;
using Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules;
using Tycho.Apps;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT;

// Handles
public record Request(TestResult Result) : IRequest;
public record RequestWithResponse(TestResult Result) : IRequest<string>;

public class TestApp(TestWorkflow<TestResult> testWorkflow) : TychoApp
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    protected override void DefineContract(IAppContract app)
    {
        app.Forwards<Request, AlphaModule>()
           .Forwards<RequestWithResponse, string, AlphaModule>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<AlphaModule>(contract =>
        {
            contract.Forward<Request, BetaModule>()
                    .Forward<RequestWithResponse, string, BetaModule>();
        });

        app.Uses<BetaModule>(contract =>
        {
            contract.Forward<Request, GammaModule>()
                    .Forward<RequestWithResponse, string, GammaModule>();
        });

        app.Uses<GammaModule>(contract =>
        {
            contract.Handle<Request, RequestHandler>()
                    .Handle<RequestWithResponse, string, RequestHandler>();
        });
    }

    protected override void MapEvents(IAppEvents app) { }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(_testWorkflow);
    }
}
