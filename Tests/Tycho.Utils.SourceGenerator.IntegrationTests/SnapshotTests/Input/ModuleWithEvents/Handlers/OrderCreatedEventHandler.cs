using Tycho.Events;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Events;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Handlers;

public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    public Task HandleAsync(EventContext<OrderCreatedEvent> context, CancellationToken ct) => throw new NotImplementedException();
}
