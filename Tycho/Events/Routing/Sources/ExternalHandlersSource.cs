using System;
using System.Collections.Generic;

namespace Tycho.Events.Routing.Sources
{
    internal abstract class ExternalHandlersSource<TEvent> : IHandlersSource
        where TEvent : class, IEvent
    {
        private readonly IEventRouter _externalEventRouter;

        public ExternalHandlersSource(IEventRouter externalEventRouter)
        {
            _externalEventRouter = externalEventRouter;
        }

        public IReadOnlyCollection<HandlerIdentity> IdentifyHandlers<TRequestedEvent>()
            where TRequestedEvent : class, IEvent
        {
            if (typeof(TRequestedEvent) == typeof(TEvent))
            {
                return _externalEventRouter.IdentifyHandlers<TEvent>();
            }

            return Array.Empty<HandlerIdentity>();
        }

        public IEventHandler? FindHandler(HandlerIdentity handlerIdentity)
        {
            if (handlerIdentity.MatchesEvent(typeof(TEvent)))
            {
                return _externalEventRouter.FindHandler(handlerIdentity);
            }

            return null;
        }
    }
}