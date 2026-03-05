using System.Collections.Generic;

namespace Tycho.Events.Routing.Routes
{
    internal class Route : Stack<IRouteStep>
    {
        public static Route Create()
        {
            var route = new Route();
            var finalRouteStep = FinalRouteStep.Create();
            route.Push(finalRouteStep);
            return route;
        }
    }
}
