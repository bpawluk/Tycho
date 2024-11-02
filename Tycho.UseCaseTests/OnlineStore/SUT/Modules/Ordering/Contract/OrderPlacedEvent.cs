using Tycho.Events;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract.OrderPlacedEvent;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;

public record OrderPlacedEvent(int CustomerId, IReadOnlyList<OrderedProduct> Items) : IEvent
{
    public record OrderedProduct(int Id, uint Quantity, decimal Price);
}