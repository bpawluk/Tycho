using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents.Events;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents.Handlers;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents;

[TychoDefinition]
public class TestModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }
    protected override void DefineEvents(IModuleEvents module)
    {
        module.Handles<OrderCreatedEvent, OrderCreatedEventHandler>();
        module.Handles<PaymentProcessedEvent, PaymentProcessedEventHandler>();
        module.Routes<PaymentFailedEvent>().Forwards<ModuleA>();
        module.Routes<PaymentFailedEvent>().Exposes();
    }
    protected override void IncludeModules(IModuleStructure module) { }
    protected override void RegisterServices(IServiceCollection module) { }
}
