using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Handlers;
using Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules;
using Tycho.Requests;
using static Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.RequestToMapWithResponse;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT;

// Handles
public record Request(TestResult Result) : IRequest;
public record RequestWithResponse(TestResult Result) : IRequest<string>;
public record RequestToMap(TestResult Result) : IRequest;
public record RequestToMapWithResponse(TestResult Result) : IRequest<Response>
{
    public record Response(string Value);
}

public class TestApp(TestWorkflow<TestResult> testWorkflow) : TychoApp
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    protected override void DefineContract(IAppContract app)
    {
        app.Forwards<Request, AlphaModule>()
           .Forwards<RequestWithResponse, string, AlphaModule>();

        app.ForwardsAs<RequestToMap, AlphaRequest, AlphaModule>(
                requestData => new(requestData.Result))
           .ForwardsAs<RequestToMapWithResponse, Response, AlphaRequestWithResponse, string, AlphaModule>(
                requestData => new(requestData.Result),
                response => new(response));
    }

    protected override void DefineEvents(IAppEvents app) { }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<AlphaModule>(contract =>
        {
            contract.Handle<Request, RequestHandler>()
                    .Handle<RequestWithResponse, string, RequestHandler>();

            contract.Handle<AlphaRequest, AlphaRequestHandler>()
                    .Handle<AlphaRequestWithResponse, string, AlphaRequestHandler>();
        });
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(_testWorkflow);
    }
}