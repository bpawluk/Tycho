using System.Collections.Generic;

namespace Tycho.Events.Routing
{
    internal interface IHandlersSource
    {
        IReadOnlyCollection<HandlerIdentity> IdentifyHandlers<TEvent>()
            where TEvent : class, IEvent;

        IEventHandler? FindHandler(HandlerIdentity handlerIdentity);
    }
}
