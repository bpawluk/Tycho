using Tycho.Identity.Modules;

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
            var moduleIdentity = ModuleIdentity.Create<TModule>();
            return new DownStreamRouteStep(moduleIdentity);
        }
    }
}