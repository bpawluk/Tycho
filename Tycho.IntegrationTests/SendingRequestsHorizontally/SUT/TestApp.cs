﻿using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Handlers;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Alpha;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Beta;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Gamma;
using Tycho.IntegrationTests.TestUtils;
using TychoV2.Apps;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT;

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
        app.AddModule<AlphaModule>(contract =>
        {
            contract.Handle<AlphaOutRequest, AlphaOutRequestHandler>()
                    .Handle<AlphaOutRequestWithResponse, string, AlphaOutRequestHandler>();
        });

        app.AddModule<BetaModule>(contract =>
        {
            contract.Handle<BetaOutRequest, BetaOutRequestHandler>()
                    .Handle<BetaOutRequestWithResponse, string, BetaOutRequestHandler>();
        });

        app.AddModule<GammaModule>(contract =>
        {
            contract.Handle<GammaOutRequest, GammaOutRequestHandler>()
                    .Handle<GammaOutRequestWithResponse, string, GammaOutRequestHandler>();
        });
    }

    protected override void MapEvents(IAppEvents app) { }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(_testWorkflow);
    }
}