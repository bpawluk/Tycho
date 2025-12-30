using Tycho.Registry;

namespace Tycho.Events.Routing.Routes
{
    internal class FinalRouteStep : IRouteStep
    {
        public EventHandlerIdentity HandlerId { get; }

        private FinalRouteStep(EventHandlerIdentity handlerId)
        {
            HandlerId = handlerId;
        }

        public static FinalRouteStep Create(EventHandlerIdentity handlerId)
        {
            return new FinalRouteStep(handlerId);
        }
    }
}
