using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public IHandle<TEvent> GetHandler<TEvent>(HandlerIdentity handlerIdentity)
            where TEvent : class, IEvent
        {
            throw new NotImplementedException();
        }
    }
}
