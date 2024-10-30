using Tycho.Events;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Handlers;

internal class OrderPlacedEventHandler : IEventHandler<OrderPlacedEvent>
{
    public Task Handle(OrderPlacedEvent eventData, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}