using Tycho.Events;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Outgoing;

public record BasketCheckedOutEvent(int CustomerId, decimal Total) : IEvent;