using Tycho.Apps;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Messaging.Routers;

internal static class InventoryEventsRouter
{
    public static IAppEvents RoutesInventoryEvents(this IAppEvents app)
    {
        app.Routes<StockLevelChangedEvent>()
           .ForwardsAs<ProductAvailabilityChangedEvent, CatalogModule>(Map);

        return app;
    }

    private static ProductAvailabilityChangedEvent Map(StockLevelChangedEvent eventData)
    {
        return new(eventData.ItemId, eventData.NewQuantity, eventData.Version);
    }
}