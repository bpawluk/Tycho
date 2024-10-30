using Tycho.Apps;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Messaging.Forwarders;

internal static class OrderingRequestsForwarder
{
    public static IAppContract ForwardsOrderingRequests(this IAppContract app)
    {
        app.Forwards<GetOrdersRequest, GetOrdersRequest.Response, OrderingModule>();

        return app;
    }
}