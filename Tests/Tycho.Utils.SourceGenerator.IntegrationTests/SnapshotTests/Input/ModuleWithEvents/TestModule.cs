using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Events;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Handlers;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents;

[TychoDefinition]
public class TestModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }
    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<OrderCreatedEvent>().HandlesWith<OrderCreatedEventHandler>();
        module.Expects<PaymentProcessedEvent>().HandlesWith<PaymentProcessedEventHandler>();
        module.Expects<PaymentFailedEvent>().ForwardsTo<ModuleA>();
        module.Expects<PaymentFailedEvent>().Exposes();
    }
    protected override void IncludeModules(IModuleStructure module) { }
    protected override void RegisterServices(IServiceCollection module) { }
}
