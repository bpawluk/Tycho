using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha.Handlers;
using TychoV2.Modules;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha;

internal class AlphaModule : TychoModule
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
