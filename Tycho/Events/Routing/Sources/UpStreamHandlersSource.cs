using System;
using Tycho.Structure;

namespace Tycho.Events.Routing.Sources
{
    internal class UpStreamHandlersSource<TEvent>
        : ExternalHandlersSource<TEvent>
        where TEvent : class, IEvent
    {
        public UpStreamHandlersSource(IParent parent)
            : base(parent.EventRouter)
        {
        }
    }

    internal class UpStreamMappedHandlersSource<TEvent, TTargetEvent>
        : MappedExternalHandlersSource<TEvent, TTargetEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        public UpStreamMappedHandlersSource(IParent parent, Func<TEvent, TTargetEvent> map)
            : base(parent.EventRouter, map)
        {
        }
    }
}