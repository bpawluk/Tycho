using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Beta.Handlers;
using Tycho.Modules;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Beta;

// Events
public record BetaWorkflowStartedEvent(TestResult Result) : IEvent;
public record BetaWorkflowFinishedEvent(TestResult Result) : IEvent;

[TychoDefinition]
public class BetaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<WorkflowStartedEvent>()
              .HandlesWith<WorkflowStartedEventHandler>();

        module.Expects<WorkflowFinishedEvent>()
              .Exposes();

        module.Expects<BetaWorkflowStartedEvent>()
              .HandlesWith<BetaWorkflowStartedEventHandler>();

        module.Expects<BetaWorkflowFinishedEvent>()
              .Exposes();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
