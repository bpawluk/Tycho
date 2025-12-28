using System;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.Events.Handling;
using Tycho.Structure.Internal;

namespace Tycho.Identities.Providers
{
    internal class EventHandlerProvider : IEventHandlerProvider
    {
        private readonly Internals _internals;

        public EventHandlerProvider(Internals internals)
        {
            _internals = internals;
        }

        public IEventHandler GetHandler(EventHandlerIdentity eventHandlerId)
        {
            var handlers = _internals.GetServices<IEventHandler>();
            foreach (var handler in handlers)
            {
                if (eventHandlerId.MatchesEvent(handler.EventType) 
                 && eventHandlerId.MatchesHandler(handler is IEventHandlerWrapper wrapper 
                    ? wrapper.InnerHandlerType 
                    : handler.HandlerType))
                {
                    return handler;
                }
            }
            throw new InvalidOperationException($"No event handler found for identity {eventHandlerId}");
        }
    }
}
