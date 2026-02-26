using System;
using Tycho.Events.Routing.Routes;
using Tycho.Structure.External;

namespace Tycho.Events.Routing.Sources
{
    internal class UpStreamRouteSource<TEvent>
        : ExternalRouteSource<TEvent>
        where TEvent : class, IEvent
    {
        public UpStreamRouteSource(IParent parent)
            : base(parent.EventRouter)
        {
        }

        protected override IRouteStep GetRouteStep() => UpStreamRouteStep.Create();
    }

    internal class UpStreamMappedRouteSource<TEvent, TTargetEvent>
        : MappedExternalRouteSource<TEvent, TTargetEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        public UpStreamMappedRouteSource(IParent parent, Func<TEvent, TTargetEvent> map)
            : base(parent.EventRouter, map)
        {
        }

        protected override IRouteStep GetRouteStep() => UpStreamRouteStep.Create();
    }
}
