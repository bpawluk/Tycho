using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Handlers;
using Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules;
using TychoV2.Apps;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT;

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
        app.AddModule<AlphaModule>(contract =>
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
