using Tycho.Apps;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Messaging;

internal static class OrderingRequestsForwarder
{
    public static IAppContract ForwardsOrderingRequests(this IAppContract app)
    {
        return app;
    }
}