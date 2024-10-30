using Tycho.Apps;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Messaging;

internal static class BasketRequestsForwarder
{
    public static IAppContract ForwardsBasketRequests(this IAppContract app)
    {
        return app;
    }
}