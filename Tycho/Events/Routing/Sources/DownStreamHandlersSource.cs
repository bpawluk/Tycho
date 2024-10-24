using System;
using Tycho.Modules;
using Tycho.Structure;

namespace Tycho.Events.Routing.Sources
{
    internal class DownStreamHandlersSource<TEvent, TModule>
        : ExternalHandlersSource<TEvent>
        where TEvent : class, IEvent
        where TModule : TychoModule
    {
        public DownStreamHandlersSource(IModule<TModule> submodule)
            : base(submodule.EventRouter)
        {
        }
    }

    internal class DownStreamMappedHandlersSource<TEvent, TTargetEvent, TModule>
        : MappedExternalHandlersSource<TEvent, TTargetEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
        where TModule : TychoModule
    {
        public DownStreamMappedHandlersSource(IModule<TModule> submodule, Func<TEvent, TTargetEvent> map)
            : base(submodule.EventRouter, map)
        {
        }
    }
}