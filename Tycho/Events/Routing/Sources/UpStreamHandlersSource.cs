using System;
using System.Collections.Generic;
using Tycho.Structure;

namespace Tycho.Events.Routing.Sources
{
    internal class UpStreamHandlersSource<TEvent> : IHandlersSource
        where TEvent : class, IEvent
    {
        private readonly IEventRouter _parentEventRouter;

        public UpStreamHandlersSource(IParent parent)
        {
            _parentEventRouter = parent.EventRouter;
        }

        public IReadOnlyCollection<HandlerIdentity> IdentifyHandlers<TRequestedEvent>()
            where TRequestedEvent : class, IEvent
        {
            if (typeof(TRequestedEvent) == typeof(TEvent))
            {
                return _parentEventRouter.IdentifyHandlers<TEvent>();
            }

            return Array.Empty<HandlerIdentity>();
        }

        public IEventHandler? FindHandler(HandlerIdentity handlerIdentity)
        {
            if (handlerIdentity.MatchesEvent(typeof(TEvent)))
            {
                return _parentEventRouter.FindHandler(handlerIdentity);
            }

            return null;
        }
    }
}