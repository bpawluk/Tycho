using System.Collections.Generic;
using Tycho.Registry;

namespace Tycho.Events.Routing.Routes
{
    internal class Route : Stack<IRouteStep>
    {
        public static Route WithHandler(EventHandlerIdentity handlerId)
        {
            var route = new Route();
            route.Push(FinalRouteStep.Create(handlerId));
            return route;
        }
    }
}
