using Tycho.Events;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Events;

public record BasketItemAddedEvent(
    int CustomerId,
    int ProductId,
    uint Quantity) 
    : IEvent;