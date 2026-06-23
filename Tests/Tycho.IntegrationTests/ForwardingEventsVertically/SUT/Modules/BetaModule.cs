using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.Modules;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules;

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
              .ForwardsTo<GammaModule>();

        module.Expects<WorkflowFinishedEvent>()
              .Exposes();

        module.Expects<BetaWorkflowStartedEvent>()
              .MapsTo<GammaWorkflowStartedEvent>(eventData => new(eventData.Result))
              .ForwardsTo<GammaModule>();

        module.Expects<BetaWorkflowFinishedEvent>()
              .MapsTo<AlphaWorkflowFinishedEvent>(eventData => new(eventData.Result))
              .Exposes();
    }

    protected override void IncludeModules(IModuleStructure module)
    {
        module.Uses<GammaModule>();
    }

    protected override void RegisterServices(IServiceCollection module) { }
}
