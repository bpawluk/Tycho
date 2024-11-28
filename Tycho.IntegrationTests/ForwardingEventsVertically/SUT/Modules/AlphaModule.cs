using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.Modules;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules;

// Events
public record AlphaWorkflowStartedEvent(TestResult Result) : IEvent;
public record AlphaWorkflowFinishedEvent(TestResult Result) : IEvent;

internal class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void IncludeModules(IModuleStructure module)
    {
        module.Uses<BetaModule>();
    }

    protected override void MapEvents(IModuleEvents module)
    {
        module.Routes<WorkflowStartedEvent>()
              .Forwards<BetaModule>();

        module.Routes<WorkflowFinishedEvent>()
              .Exposes();

        module.Routes<AlphaWorkflowStartedEvent>()
              .ForwardsAs<BetaWorkflowStartedEvent, BetaModule>(
                  eventData => new(eventData.Result));

        module.Routes<AlphaWorkflowFinishedEvent>()
              .ExposesAs<WorkflowWithMappingFinishedEvent>(
                  eventData => new(eventData.Result));
    }

    protected override void RegisterServices(IServiceCollection module) { }
}