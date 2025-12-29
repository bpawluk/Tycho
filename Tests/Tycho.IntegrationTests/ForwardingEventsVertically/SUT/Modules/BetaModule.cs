using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.Modules;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules;

// Events
public record BetaWorkflowStartedEvent(TestResult Result) : IEvent;
public record BetaWorkflowFinishedEvent(TestResult Result) : IEvent;

internal class BetaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Routes<WorkflowStartedEvent>()
              .Forwards<GammaModule>();

        module.Routes<WorkflowFinishedEvent>()
              .Exposes();

        module.Routes<BetaWorkflowStartedEvent>()
              .ForwardsAs<GammaWorkflowStartedEvent, GammaModule>(
                  eventData => new(eventData.Result));

        module.Routes<BetaWorkflowFinishedEvent>()
              .ExposesAs<AlphaWorkflowFinishedEvent>(
                  eventData => new(eventData.Result));
    }

    protected override void IncludeModules(IModuleStructure module)
    {
        module.Uses<GammaModule>();
    }

    protected override void RegisterServices(IServiceCollection module) { }
}