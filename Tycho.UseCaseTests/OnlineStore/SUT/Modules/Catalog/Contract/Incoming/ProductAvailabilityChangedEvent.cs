using Tycho.Events;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;

public record ProductAvailabilityChangedEvent(int Product, uint NewQuantity, uint Version) : IEvent;