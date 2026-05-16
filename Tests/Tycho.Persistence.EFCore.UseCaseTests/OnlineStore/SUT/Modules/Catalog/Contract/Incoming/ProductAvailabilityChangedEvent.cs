using Tycho.Events;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;

public record ProductAvailabilityChangedEvent(int Product, uint NewQuantity, uint Version) : IEvent;
