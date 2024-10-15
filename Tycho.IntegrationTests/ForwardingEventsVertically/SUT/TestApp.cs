using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Events;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Handlers;
using Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT;

// Handles
public record BeginTestWorkflowRequest(TestResult Result) : IRequest;

// Events
public record WorkflowStartedEvent(TestResult Result) : IEvent;
public record WorkflowFinishedEvent(TestResult Result) : IEvent;

internal class TestApp(TestWorkflow<TestResult> testWorkflow) : TychoApp
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    protected override void DefineContract(IAppContract app)
    {
        app.Handles<BeginTestWorkflowRequest, BeginTestWorkflowRequestHandler>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<AlphaModule>();
    }

    protected override void MapEvents(IAppEvents app)
    {
        app.Routes<WorkflowStartedEvent>()
           .Forwards<AlphaModule>();

        app.Handles<WorkflowFinishedEvent, WorkflowFinishedEventHandler>();
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(_testWorkflow);
    }
}