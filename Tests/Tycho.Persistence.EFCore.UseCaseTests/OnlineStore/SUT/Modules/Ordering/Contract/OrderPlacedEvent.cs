using Tycho.Events;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;

public record OrderPlacedEvent(int CustomerId, decimal Total) : IEvent;
