using Tycho.Apps;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Messaging;

internal static class BasketEventsRouter
{
    public static IAppEvents RoutesBasketEvents(this IAppEvents app)
    {
        return app;
    }
}