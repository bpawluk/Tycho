using System;
using System.Collections.Generic;
using System.Linq;
using Tycho.Events.Routing.Steps;

namespace Tycho.Events.Routing
{
    internal class Route : Stack<IRouteStep>
    {
        private const string _separator = "/";

        private Route() : base() { }

        public static Route Empty()
        {
            return new Route();
        }

        public static Route Create()
        {
            var route = new Route();
            var finalRouteStep = FinalRouteStep.Create();
            route.Push(finalRouteStep);
            return route;
        }

        public override string ToString()
        {
            return string.Join(_separator, this);
        }

        public static Route Parse(string route)
        {
            var result = new Route();
            IEnumerable<string> parts = route.Split(_separator).Reverse();

            foreach (string part in parts)
            {
                if (FinalRouteStep.TryParse(part, out IRouteStep? finalStep))
                {
                    result.Push(finalStep);
                    continue;
                }

                if (DownStreamRouteStep.TryParse(part, out IRouteStep? downStep))
                {
                    result.Push(downStep);
                    continue;
                }

                if (UpStreamRouteStep.TryParse(part, out IRouteStep? upStep))
                {
                    result.Push(upStep);
                    continue;
                }

                throw new FormatException($"Invalid {nameof(Route)} format: {route}");
            }

            return result;
        }
    }
}
