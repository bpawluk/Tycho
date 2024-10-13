using System.Collections.Generic;
using Tycho.Events.Routing;

namespace Tycho.Events.Handling
{
    internal interface IHandlersSource
    {
        IReadOnlyCollection<HandlerIdentity> IdentifyHandlers<TEvent>()
            where TEvent : class, IEvent;

        IEventHandler? FindHandler(HandlerIdentity handlerIdentity);
    }
}
