using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Beta.Handlers;
using Tycho.Modules;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Beta;

// Events
public record BetaWorkflowStartedEvent(TestResult Result) : IEvent;
public record BetaWorkflowFinishedEvent(TestResult Result) : IEvent;

internal class BetaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module)
    {
        module.Handles<WorkflowStartedEvent, WorkflowStartedEventHandler>();

        module.Routes<WorkflowFinishedEvent>()
              .Exposes();

        module.Handles<BetaWorkflowStartedEvent, BetaWorkflowStartedEventHandler>();

        module.Routes<BetaWorkflowFinishedEvent>()
              .Exposes();
    }

    protected override void RegisterServices(IServiceCollection module) { }
}