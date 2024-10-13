using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules.Handlers;
using Tycho.Modules;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules;

internal class GammaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module)
    {
        module.Handles<WorkflowStartedEvent, WorkflowStartedEventHandler>();

        module.Routes<WorkflowFinishedEvent>()
              .Exposes();
    }

    protected override void RegisterServices(IServiceCollection module) { }
}
