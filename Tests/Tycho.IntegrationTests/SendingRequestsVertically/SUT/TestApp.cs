using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Handlers;
using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Alpha;
using Tycho.IntegrationTests._Utils;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT;

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
        app.Uses<AlphaModule>(contract =>
        {
            contract.Handle<AlphaOutRequest, AlphaOutRequestHandler>()
                    .Handle<AlphaOutRequestWithResponse, string, AlphaOutRequestHandler>();
        });
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(_testWorkflow);
    }
}
