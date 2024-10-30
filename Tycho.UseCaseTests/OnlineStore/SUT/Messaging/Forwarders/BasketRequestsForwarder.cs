using Tycho.Apps;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Requests;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Messaging.Forwarders;

internal static class BasketRequestsForwarder
{
    public static IAppContract ForwardsBasketRequests(this IAppContract app)
    {
        app.Forwards<AddBasketItemRequest, BasketModule>()
           .Forwards<CheckoutRequest, BasketModule>();

        return app;
    }
}