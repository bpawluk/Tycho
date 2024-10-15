using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Structure;

namespace Tycho.Events.Routing
{
    internal interface IEventRouter
    {
        IReadOnlyCollection<HandlerIdentity> IdentifyHandlers<TEvent>()
            where TEvent : class, IEvent;

        IEventHandler? FindHandler(HandlerIdentity handlerIdentity);
    }

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
            var sources = _internals.GetServices<IHandlersSource>();
            return sources.SelectMany(source => source.IdentifyHandlers<TEvent>()).ToArray();
        }

        public IEventHandler? FindHandler(HandlerIdentity handlerIdentity)
        {
            foreach (var source in _internals.GetServices<IHandlersSource>())
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