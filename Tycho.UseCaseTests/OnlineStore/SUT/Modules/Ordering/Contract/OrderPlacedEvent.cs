using Tycho.Events;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;

public record OrderPlacedEvent(int CustomerId, decimal Total) : IEvent;