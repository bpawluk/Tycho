using System;
using Tycho.Events.Routing.Routes;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Events.Routing.Sources
{
    internal class DownStreamRouteSource<TEvent, TModule>
        : ExternalRouteSource<TEvent>
        where TEvent : class, IEvent
        where TModule : TychoModule
    {
        public DownStreamRouteSource(IModule<TModule> submodule) : base(submodule.EventRouter)
        {
        }

        protected override RouteStep GetRouteStep() => DownStreamRouteStep.Create<TModule>();
    }

    internal class DownStreamMappedRouteSource<TEvent, TTargetEvent, TModule>
        : MappedExternalRouteSource<TEvent, TTargetEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
        where TModule : TychoModule
    {
        public DownStreamMappedRouteSource(IModule<TModule> submodule, Func<TEvent, TTargetEvent> map) : base(submodule.EventRouter, map)
        {
        }

        protected override RouteStep GetRouteStep() => DownStreamRouteStep.Create<TModule>();
    }
}
