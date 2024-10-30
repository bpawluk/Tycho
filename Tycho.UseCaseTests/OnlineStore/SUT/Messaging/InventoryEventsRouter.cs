using Tycho.Apps;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Messaging;

internal static class InventoryEventsRouter
{
    public static IAppEvents RoutesInventoryEvents(this IAppEvents app)
    {
        return app;
    }
}