using System;
using System.Collections.Generic;
using Tycho.Modules;

namespace Tycho.Events.Routing.Sources
{
    internal class DownStreamHandlersSource<TEvent, TModule> : IHandlersSource
        where TEvent : class, IEvent
        where TModule : TychoModule
    {
        private readonly IEventRouter _submoduleEventRouter;

        public DownStreamHandlersSource(IModule<TModule> submodule)
        {
            _submoduleEventRouter = submodule.EventRouter;
        }

        public IReadOnlyCollection<HandlerIdentity> IdentifyHandlers<TRequestedEvent>()
            where TRequestedEvent : class, IEvent
        {
            if (typeof(TRequestedEvent) == typeof(TEvent))
            {
                return _submoduleEventRouter.IdentifyHandlers<TEvent>();
            }
            return Array.Empty<HandlerIdentity>();
        }

        public IEventHandler? FindHandler(HandlerIdentity handlerIdentity)
        {
            if (handlerIdentity.MatchesEvent(typeof(TEvent)))
            {
                return _submoduleEventRouter.FindHandler(handlerIdentity);
            }
            return null;
        }
    }
}
