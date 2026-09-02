using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithEvents.Events;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithEvents.Handlers;

public class PaymentProcessedEventHandler : IEventHandler<PaymentProcessedEvent>
{
    public Task HandleAsync(EventContext<PaymentProcessedEvent> context, CancellationToken ct) => throw new NotImplementedException();
}
