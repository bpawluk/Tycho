using Basket = Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Events;
using Catalog = Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;
using Inventory = Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract;
using Ordering = Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Mappers;

internal static class EventMapper
{
    public static Ordering.OrderPlacedEvent Map(Basket.BasketCheckedOutEvent eventData)
    {
        return new(eventData.CustomerId, eventData.Items
            .Select(item =>
                new Ordering.OrderPlacedEvent.OrderedProduct(
                    item.ProductId,
                    item.Quantity,
                    item.Price))
            .ToArray());
    }

    public static Catalog.ProductAvailabilityChangedEvent Map(Inventory.ItemAvailabilityChangedEvent eventData)
    {
        return new(eventData.ItemId, eventData.NewQuantity, eventData.Version);
    }
}
