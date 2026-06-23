using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Events;
using Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Handlers;
using Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules;
using Tycho.IntegrationTests._Utils;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT;

// Handles
public record BeginTestWorkflowRequest(TestResult Result) : IRequest;

// Events
public record WorkflowStartedEvent(TestResult Result) : IEvent;
public record WorkflowFinishedEvent(TestResult Result) : IEvent;
public record WorkflowWithMappingStartedEvent(TestResult Result) : IEvent;
public record WorkflowWithMappingFinishedEvent(TestResult Result) : IEvent;

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
           .ForwardsTo<AlphaModule>();

        app.Expects<WorkflowFinishedEvent>()
           .HandlesWith<WorkflowFinishedEventHandler>();

        app.Expects<WorkflowWithMappingStartedEvent>()
           .MapsTo<AlphaWorkflowStartedEvent>(eventData => new(eventData.Result))
           .ForwardsTo<AlphaModule>();

        app.Expects<WorkflowWithMappingFinishedEvent>()
           .HandlesWith<WorkflowWithMappingFinishedEventHandler>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<AlphaModule>();
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(_testWorkflow);
    }
}
