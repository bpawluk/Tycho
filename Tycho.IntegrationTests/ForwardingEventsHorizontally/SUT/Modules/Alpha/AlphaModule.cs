using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha.Handlers;
using Tycho.Modules;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha;

// Events
public record AlphaWorkflowStartedEvent(TestResult Result) : IEvent;
public record AlphaWorkflowFinishedEvent(TestResult Result) : IEvent;

internal class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module)
    {
        module.Handles<WorkflowStartedEvent, WorkflowStartedEventHandler>();

        module.Routes<WorkflowFinishedEvent>()
              .Exposes();

        module.Handles<AlphaWorkflowStartedEvent, AlphaWorkflowStartedEventHandler>();

        module.Routes<AlphaWorkflowFinishedEvent>()
              .Exposes();
    }

    protected override void RegisterServices(IServiceCollection module) { }
}