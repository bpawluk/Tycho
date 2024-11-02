using Tycho.Events;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Events.BasketCheckedOutEvent;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Events;

public record BasketCheckedOutEvent(int CustomerId, IReadOnlyList<BasketItem> Items) : IEvent
{
    public record BasketItem(int ProductId, uint Quantity, decimal Price);
}