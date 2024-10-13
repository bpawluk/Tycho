using Microsoft.Extensions.DependencyInjection;
using TychoV2.Modules;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules;

internal class BetaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void IncludeModules(IModuleStructure module)
    {
        module.Uses<GammaModule>();
    }

    protected override void MapEvents(IModuleEvents module)
    {
        module.Routes<WorkflowStartedEvent>()
              .Forwards<GammaModule>();

        module.Routes<WorkflowFinishedEvent>()
              .Exposes();
    }

    protected override void RegisterServices(IServiceCollection module) { }
}
