using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Gamma.Handlers;
using Tycho.Modules;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Gamma;

// Events
public record GammaWorkflowStartedEvent(TestResult Result) : IEvent;
public record GammaWorkflowFinishedEvent(TestResult Result) : IEvent;

[TychoDefinition]
public class GammaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<WorkflowStartedEvent>()
              .HandlesWith<WorkflowStartedEventHandler>();

        module.Expects<WorkflowFinishedEvent>()
              .Exposes();

        module.Expects<GammaWorkflowStartedEvent>()
              .HandlesWith<GammaWorkflowStartedEventHandler>();

        module.Expects<GammaWorkflowFinishedEvent>()
              .Exposes();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
