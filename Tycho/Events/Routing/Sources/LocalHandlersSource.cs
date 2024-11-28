using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Structure.Internal;

namespace Tycho.Events.Routing.Sources
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
                        handler.HandlerType,
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
                    .FirstOrDefault(handler => handlerIdentity.MatchesHandler(handler.HandlerType));

                if (handler is null)
                {
                    throw new InvalidOperationException($"Missing event handler with ID {handlerIdentity}");
                }

                return handler; 
            }

            return null;
        }
    }
}