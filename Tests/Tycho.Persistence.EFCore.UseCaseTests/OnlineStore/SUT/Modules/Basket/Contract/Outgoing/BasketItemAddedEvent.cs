using Tycho.Events;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Outgoing;

public record BasketItemAddedEvent(
    int CustomerId,
    int ProductId,
    uint Quantity)
    : IEvent;
