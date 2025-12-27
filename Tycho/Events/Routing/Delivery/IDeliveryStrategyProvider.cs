using Tycho.Events.Routing.Routes;

namespace Tycho.Events.Routing.Delivery
{
    internal interface IDeliveryStrategyProvider
    {
        IDeliveryStrategy GetDeliveryStrategy(IRouteStep routeStep);
    }
}
