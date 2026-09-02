using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents.Events;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents.Handlers;

public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    public Task HandleAsync(EventContext<OrderCreatedEvent> context, CancellationToken ct) => throw new NotImplementedException();
}
