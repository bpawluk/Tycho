using Tycho.Events.Routing.Routes;

namespace Tycho.Events.Delivery
{
    internal interface IDeliveryStrategyProvider
    {
        IDeliveryStrategy GetDeliveryStrategy(RouteStep routeStep);
    }
}
