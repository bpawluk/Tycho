using System.Collections.Generic;
using TychoV2.Events.Routing;

namespace TychoV2.Events.Handling
{
    internal interface IHandlersSource
    {
        IReadOnlyCollection<HandlerIdentity> IdentifyHandlers<TEvent>()
            where TEvent : class, IEvent;

        IEventHandler? FindHandler(HandlerIdentity handlerIdentity);
    }
}
