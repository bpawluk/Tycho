using System;
using System.Collections.Generic;
using System.Linq;
using Tycho.Events.Handling;

namespace Tycho.Events.Routing.Sources
{
    internal abstract class MappedExternalHandlersSource<TEvent, TTargetEvent> : IHandlersSource
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        private readonly IEventRouter _externalEventRouter;
        private readonly Func<TEvent, TTargetEvent> _map;

        public MappedExternalHandlersSource(IEventRouter externalEventRouter, Func<TEvent, TTargetEvent> map)
        {
            _externalEventRouter = externalEventRouter;
            _map = map;
        }

        public IReadOnlyCollection<HandlerIdentity> IdentifyHandlers<TRequestedEvent>()
            where TRequestedEvent : class, IEvent
        {
            if (typeof(TRequestedEvent) == typeof(TEvent))
            {
                var foundIdentities = _externalEventRouter.IdentifyHandlers<TTargetEvent>();
                var mappedIdentities = foundIdentities.Select(id => id.ForEvent(typeof(TEvent)));
                return mappedIdentities.ToArray();
            }

            return Array.Empty<HandlerIdentity>();
        }

        public IEventHandler? FindHandler(HandlerIdentity handlerIdentity)
        {
            if (handlerIdentity.MatchesEvent(typeof(TEvent)))
            {
                var mappedIdentity = handlerIdentity.ForEvent(typeof(TTargetEvent));
                var foundHandler = _externalEventRouter.FindHandler(mappedIdentity);
                if (foundHandler != null)
                {
                    return new WrappingHandler<TEvent, TTargetEvent>(
                        (foundHandler as IEventHandler<TTargetEvent>)!,
                        _map);
                }
            }

            return null;
        }
    }
}