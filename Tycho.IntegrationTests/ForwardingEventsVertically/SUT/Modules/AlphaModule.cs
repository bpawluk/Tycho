using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules;

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
    }

    protected override void RegisterServices(IServiceCollection module) { }
}