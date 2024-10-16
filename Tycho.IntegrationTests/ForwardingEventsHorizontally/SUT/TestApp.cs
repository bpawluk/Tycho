using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Events;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Handlers;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Beta;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Gamma;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT;

// Handles
public record BeginTestWorkflowRequest(TestResult Result) : IRequest;

// Events
public record WorkflowStartedEvent(TestResult Result) : IEvent;
public record WorkflowFinishedEvent(TestResult Result, Type FinalModule) : IEvent;
public record WorkflowWithMappingStartedEvent(TestResult Result) : IEvent;

internal class TestApp(TestWorkflow<TestResult> testWorkflow) : TychoApp
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    protected override void DefineContract(IAppContract app)
    {
        app.Handles<BeginTestWorkflowRequest, BeginTestWorkflowRequestHandler>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<AlphaModule>()
           .Uses<BetaModule>()
           .Uses<GammaModule>();
    }

    protected override void MapEvents(IAppEvents app)
    {
        app.Routes<WorkflowStartedEvent>()
           .Forwards<AlphaModule>()
           .Forwards<BetaModule>()
           .Forwards<GammaModule>();

        app.Handles<WorkflowFinishedEvent, WorkflowFinishedEventHandler>();

        app.Routes<WorkflowWithMappingStartedEvent>()
           .ForwardsAs<AlphaWorkflowStartedEvent, AlphaModule>(
                eventData => new(eventData.Result))
           .ForwardsAs<BetaWorkflowStartedEvent, BetaModule>(
                eventData => new(eventData.Result))
           .ForwardsAs<GammaWorkflowStartedEvent, GammaModule>(
                eventData => new(eventData.Result));

        app.Handles<AlphaWorkflowFinishedEvent, AlphaWorkflowFinishedEventHandler>()
           .Handles<BetaWorkflowFinishedEvent, BetaWorkflowFinishedEventHandler>()
           .Handles<GammaWorkflowFinishedEvent, GammaWorkflowFinishedEventHandler>();
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(_testWorkflow)
           .AddSingleton(new CompoundResult<Type>([typeof(AlphaModule), typeof(BetaModule), typeof(GammaModule)]));
    }
}