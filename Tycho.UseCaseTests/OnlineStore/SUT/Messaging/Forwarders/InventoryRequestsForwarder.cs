using Tycho.Apps;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Messaging.Forwarders;

internal static class InventoryRequestsForwarder
{
    public static IAppContract ForwardsInventoryRequests(this IAppContract app)
    {
        app.Forwards<StockItemRequest, InventoryModule>();

        return app;
    }
}