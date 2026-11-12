using Tycho.Events;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Events;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Handlers;

public class PaymentProcessedEventHandler : IEventHandler<PaymentProcessedEvent>
{
    public Task HandleAsync(EventContext<PaymentProcessedEvent> context, CancellationToken ct) => throw new NotImplementedException();
}
