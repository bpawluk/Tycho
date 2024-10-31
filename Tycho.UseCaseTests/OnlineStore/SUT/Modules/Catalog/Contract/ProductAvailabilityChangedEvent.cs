using Tycho.Events;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;

public record ProductAvailabilityChangedEvent(int Product, uint NewQuantity, uint Version) : IEvent;