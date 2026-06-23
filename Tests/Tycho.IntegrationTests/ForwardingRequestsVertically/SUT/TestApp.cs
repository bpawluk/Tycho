using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Handlers;
using Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules;
using Tycho.IntegrationTests._Utils;
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
