using Tycho.Events;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract;

public record StockLevelChangedEvent() : IEvent;