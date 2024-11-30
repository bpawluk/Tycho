using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules.Handlers;
using Tycho.Modules;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules;

// Events
public record GammaWorkflowStartedEvent(TestResult Result) : IEvent;
public record GammaWorkflowFinishedEvent(TestResult Result) : IEvent;

internal class GammaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Handles<WorkflowStartedEvent, WorkflowStartedEventHandler>();

        module.Routes<WorkflowFinishedEvent>()
              .Exposes();

        module.Handles<GammaWorkflowStartedEvent, GammaWorkflowStartedEventHandler>();

        module.Routes<GammaWorkflowFinishedEvent>()
              .ExposesAs<BetaWorkflowFinishedEvent>(
                  eventData => new(eventData.Result));
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}