﻿using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Handlers;
using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Alpha;
using Tycho.Apps;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT;

// Handles
public record Request(TestResult Result) : IRequest;
public record RequestWithResponse(TestResult Result) : IRequest<string>;

public class TestApp(TestWorkflow<TestResult> testWorkflow) : TychoApp
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    protected override void DefineContract(IAppContract app)
    {
        app.Handles<Request, RequestHandler>()
           .Handles<RequestWithResponse, string, RequestHandler>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<AlphaModule>(contract =>
        {
            contract.Handle<AlphaOutRequest, AlphaOutRequestHandler>()
                    .Handle<AlphaOutRequestWithResponse, string, AlphaOutRequestHandler>();
        });
    }

    protected override void MapEvents(IAppEvents app) { }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(_testWorkflow);
    }
}
