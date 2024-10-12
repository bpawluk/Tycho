using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using TychoV2.Events.Handling;
using TychoV2.Structure;

namespace TychoV2.Events.Routing
{
    internal class EventRouter : IEventRouter
    {
        private readonly Internals _internals;

        public EventRouter(Internals internals)
        {
            _internals = internals;
        }

        public IReadOnlyCollection<HandlerIdentity> IdentifyHandlers<TEvent>()
            where TEvent : class, IEvent
        {
            var sources = _internals.GetServices<IHandlersSource<TEvent>>();
            return sources.SelectMany(source => source.IdentifyHandlers()).ToArray();
        }

        public IEventHandler<TEvent>? FindHandler<TEvent>(HandlerIdentity handlerIdentity)
            where TEvent : class, IEvent
        {
            var sources = _internals.GetServices<IHandlersSource<TEvent>>();
            foreach (var source in sources)
            {
                var handler = source.FindHandler(handlerIdentity);
                if (handler != null)
                {
                    return handler;
                }
            }
            return null;
        }
    }
}
