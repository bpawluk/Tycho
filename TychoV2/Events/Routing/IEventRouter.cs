using System.Collections.Generic;

namespace TychoV2.Events.Routing
{
    internal interface IEventRouter
    {
        IReadOnlyCollection<HandlerIdentity> IdentifyHandlers<TEvent>()
            where TEvent : class, IEvent;

        public IEventHandler<TEvent>? FindHandler<TEvent>(HandlerIdentity handlerIdentity)
            where TEvent : class, IEvent;
    }
}
