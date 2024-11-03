using Tycho.Events;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Outgoing;

public record ItemAvailabilityChangedEvent(int ItemId, uint NewQuantity, uint Version) : IEvent;