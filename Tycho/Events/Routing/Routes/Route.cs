using System.Collections.Generic;

namespace Tycho.Events.Routing.Routes
{
    internal class Route : Stack<RouteStep>
    {
        public static Route Empty()
        {
            var route = new Route();
            route.Push(new FinalRouteStep());
            return route;
        }
    }
}
