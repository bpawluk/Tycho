using Tycho.Registry;

namespace Tycho.Events.Routing.Routes
{
    internal class DownStreamRouteStep : RouteStep
    {
        public ModuleIdentity Destination { get; }

        private DownStreamRouteStep(ModuleIdentity destination)
        {
            Destination = destination;
        }

        public static DownStreamRouteStep Create<TModule>()
        {
            return new DownStreamRouteStep(new ModuleIdentity(typeof(TModule)));
        }
    }
}