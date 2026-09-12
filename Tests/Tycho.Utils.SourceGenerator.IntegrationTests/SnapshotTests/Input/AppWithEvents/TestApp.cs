using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.Events;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.Handlers;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents;

[TychoDefinition]
public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app) { }
    protected override void DefineEvents(IAppEvents app)
    {
        app.Expects<OrderCreatedEvent>().HandlesWith<OrderCreatedEventHandler>();
        app.Expects<PaymentProcessedEvent>().HandlesWith<PaymentProcessedEventHandler>();
        app.Expects<PaymentFailedEvent>().ForwardsTo<ModuleA>();
    }
    protected override void IncludeModules(IAppStructure app) { }
    protected override void RegisterServices(IServiceCollection app) { }
}
