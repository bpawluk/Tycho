using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using TychoV2.Events.Routing;
using TychoV2.Structure;

namespace TychoV2.Events.Handling
{
    internal class LocalHandlersSource<TEvent> : IHandlersSource
        where TEvent : class, IEvent
    {
        private readonly Internals _internals;

        public LocalHandlersSource(Internals internals)
        {
            _internals = internals;
        }

        public IReadOnlyCollection<HandlerIdentity> IdentifyHandlers<TRequestedEvent>()
            where TRequestedEvent : class, IEvent
        {
            if (typeof(TRequestedEvent) == typeof(TEvent))
            {
                return _internals
                    .GetServices<IEventHandler<TEvent>>()
                    .Select(handler => new HandlerIdentity(
                        typeof(TEvent),
                        handler.GetType(),
                        _internals.Owner))
                    .ToArray();
            }
            return Array.Empty<HandlerIdentity>();
        }

        public IEventHandler? FindHandler(HandlerIdentity handlerIdentity)
        {
            if (handlerIdentity.MatchesModule(_internals.Owner) && handlerIdentity.MatchesEvent(typeof(TEvent)))
            {
                var handler = _internals
                    .GetServices<IEventHandler<TEvent>>()
                    .FirstOrDefault(handler => handlerIdentity.MatchesHandler(handler.GetType()));
                return handler ?? throw new InvalidOperationException($"Missing event handler with ID {handlerIdentity}");
            }
            return null;
        }
    }
}
