using System.Collections.Generic;

namespace Tycho.Events.Routing
{
    internal interface IEventRouter
    {
        IReadOnlyCollection<HandlerIdentity> IdentifyHandlers<TEvent>()
            where TEvent : class, IEvent;

        public IEventHandler? FindHandler(HandlerIdentity handlerIdentity);
    }
}
