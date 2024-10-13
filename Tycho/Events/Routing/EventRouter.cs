using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Tycho.Events.Handling;
using Tycho.Structure;

namespace Tycho.Events.Routing
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
            var sources = _internals.GetServices<IHandlersSource>();
            return sources.SelectMany(source => source.IdentifyHandlers<TEvent>()).ToArray();
        }

        public IEventHandler? FindHandler(HandlerIdentity handlerIdentity)
        {
            var sources = _internals.GetServices<IHandlersSource>();
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
