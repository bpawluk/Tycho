using BasketOut = Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Outgoing;
using CatalogIn = Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using InventoryOut = Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Outgoing;
using OrderingIn = Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Mappers;

internal static class EventMapper
{
    public static OrderingIn.OrderPlacedEvent Map(BasketOut.BasketCheckedOutEvent eventData)
    {
        return new(eventData.CustomerId, eventData.Total);
    }

    public static CatalogIn.ProductAvailabilityChangedEvent Map(InventoryOut.ItemAvailabilityChangedEvent eventData)
    {
        return new(eventData.ItemId, eventData.NewQuantity, eventData.Version);
    }
}