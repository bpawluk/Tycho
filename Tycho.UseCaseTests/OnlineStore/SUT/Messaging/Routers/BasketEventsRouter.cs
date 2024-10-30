using Tycho.Apps;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Events;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Messaging.Routers;

internal static class BasketEventsRouter
{
    public static IAppEvents RoutesBasketEvents(this IAppEvents app)
    {
        app.Routes<BasketCheckedOutEvent>()
           .ForwardsAs<OrderPlacedEvent, OrderingModule>(Map);

        return app;
    }

    private static OrderPlacedEvent Map(BasketCheckedOutEvent eventData)
    {
        return new();
    }
}