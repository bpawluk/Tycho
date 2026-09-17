using Tycho.Events;
using Tycho.Utils.SourceGenerator.IntegrationTests.IncrementalTests.SUT.AppWithEvents.Events;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.IncrementalTests.SUT.AppWithEvents.Handlers;

public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    public Task HandleAsync(EventContext<OrderCreatedEvent> context, CancellationToken ct) => throw new NotImplementedException();
}
