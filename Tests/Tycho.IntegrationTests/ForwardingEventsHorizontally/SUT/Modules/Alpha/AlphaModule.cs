using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha.Handlers;
using Tycho.Modules;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha;

// Events
public record AlphaWorkflowStartedEvent(TestResult Result) : IEvent;
public record AlphaWorkflowFinishedEvent(TestResult Result) : IEvent;

[TychoDefinition]
public class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<WorkflowStartedEvent>()
              .HandlesWith<WorkflowStartedEventHandler>();

        module.Expects<WorkflowFinishedEvent>()
              .Exposes();

        module.Expects<AlphaWorkflowStartedEvent>()
              .HandlesWith<AlphaWorkflowStartedEventHandler>();

        module.Expects<AlphaWorkflowFinishedEvent>()
              .Exposes();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
