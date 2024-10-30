using Tycho.Apps;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Messaging;

internal static class InventoryRequestsForwarder
{
    public static IAppContract ForwardsInventoryRequests(this IAppContract app)
    {
        return app;
    }
}