using Tycho.Events;
using Tycho.Utils.SourceGenerator.IntegrationTests.IncrementalTests.SUT.AppWithEvents.Events;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.IncrementalTests.SUT.AppWithEvents.Handlers;

public class PaymentProcessedEventHandler : IEventHandler<PaymentProcessedEvent>
{
    public Task HandleAsync(EventContext<PaymentProcessedEvent> context, CancellationToken ct) => throw new NotImplementedException();
}
