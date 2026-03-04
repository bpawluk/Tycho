using System;
using Tycho.Events.Routing.Routes;
using Tycho.Structure.Parent;

namespace Tycho.Events.Routing.Sources
{
    internal class UpStreamRouteSource<TEvent>
        : ExternalRouteSource<TEvent>
        where TEvent : class, IEvent
    {
        public UpStreamRouteSource(IParentReference parent)
            : base(parent.EventBroker)
        {
        }

        protected override RouteStep GetRouteStep() => new UpStreamRouteStep();
    }

    internal class UpStreamMappedRouteSource<TEvent, TTargetEvent>
        : MappedExternalRouteSource<TEvent, TTargetEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        public UpStreamMappedRouteSource(IParentReference parent, Func<TEvent, TTargetEvent> map)
            : base(parent.EventBroker, map)
        {
        }

        protected override RouteStep GetRouteStep() => new UpStreamRouteStep();
    }
}
