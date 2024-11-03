using Tycho.Events;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Outgoing;

public record BasketItemAddedEvent(
    int CustomerId,
    int ProductId,
    uint Quantity)
    : IEvent;