using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.Modules;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules;

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
              .ForwardsTo<BetaModule>();

        module.Expects<WorkflowFinishedEvent>()
              .Exposes();

        module.Expects<AlphaWorkflowStartedEvent>()
              .MapsTo<BetaWorkflowStartedEvent>(eventData => new(eventData.Result))
              .ForwardsTo<BetaModule>();

        module.Expects<AlphaWorkflowFinishedEvent>()
              .MapsTo<WorkflowWithMappingFinishedEvent>(eventData => new(eventData.Result))
              .Exposes();
    }

    protected override void IncludeModules(IModuleStructure module)
    {
        module.Uses<BetaModule>();
    }

    protected override void RegisterServices(IServiceCollection module) { }
}
