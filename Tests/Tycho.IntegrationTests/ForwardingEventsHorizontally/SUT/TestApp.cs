using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Events;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Handlers;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Beta;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Gamma;
using Tycho.IntegrationTests._Utils;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT;

// Handles
public record BeginTestWorkflowRequest(TestResult Result) : IRequest;

// Events
public record WorkflowStartedEvent(TestResult Result) : IEvent;
public record WorkflowFinishedEvent(TestResult Result, string FinalModule) : IEvent;
public record WorkflowWithMappingStartedEvent(TestResult Result) : IEvent;

[TychoDefinition]
public class TestApp(TestWorkflow<TestResult> testWorkflow) : TychoApp
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    protected override void DefineContract(IAppContract app)
    {
        app.Handles<BeginTestWorkflowRequest, BeginTestWorkflowRequestHandler>();
    }

    protected override void DefineEvents(IAppEvents app)
    {
        app.Expects<WorkflowStartedEvent>()
           .ForwardsTo<AlphaModule>()
           .ForwardsTo<BetaModule>()
           .ForwardsTo<GammaModule>();

        app.Expects<WorkflowFinishedEvent>()
           .HandlesWith<WorkflowFinishedEventHandler>();

        app.Expects<WorkflowWithMappingStartedEvent>()
           .MapsTo<AlphaWorkflowStartedEvent>(eventData => new(eventData.Result))
           .ForwardsTo<AlphaModule>();

        app.Expects<WorkflowWithMappingStartedEvent>()
           .MapsTo<BetaWorkflowStartedEvent>(eventData => new(eventData.Result))
           .ForwardsTo<BetaModule>();

        app.Expects<WorkflowWithMappingStartedEvent>()
           .MapsTo<GammaWorkflowStartedEvent>(eventData => new(eventData.Result))
           .ForwardsTo<GammaModule>();

        app.Expects<AlphaWorkflowFinishedEvent>()
           .HandlesWith<AlphaWorkflowFinishedEventHandler>();

        app.Expects<BetaWorkflowFinishedEvent>()
           .HandlesWith<BetaWorkflowFinishedEventHandler>();

        app.Expects<GammaWorkflowFinishedEvent>()
           .HandlesWith<GammaWorkflowFinishedEventHandler>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<AlphaModule>()
           .Uses<BetaModule>()
           .Uses<GammaModule>();
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(_testWorkflow)
           .AddSingleton(new CompoundResult<string>([nameof(AlphaModule), nameof(BetaModule), nameof(GammaModule)]));
    }
}
