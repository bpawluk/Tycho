using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithEvents.Events;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithEvents.Handlers;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithEvents.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithEvents;

[TychoDefinition]
public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app) { }
    protected override void DefineEvents(IAppEvents app)
    {
        app.Handles<OrderCreatedEvent, OrderCreatedEventHandler>();
        app.Handles<PaymentProcessedEvent, PaymentProcessedEventHandler>();
        app.Routes<PaymentFailedEvent>().Forwards<ModuleA>();
    }
    protected override void IncludeModules(IAppStructure app) { }
    protected override void RegisterServices(IServiceCollection app) { }
}
