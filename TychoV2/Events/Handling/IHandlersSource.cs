using System.Collections.Generic;
using TychoV2.Events.Routing;

namespace TychoV2.Events.Handling
{
    internal interface IHandlersSource<TEvent>
        where TEvent : class, IEvent
    {
        IReadOnlyCollection<HandlerIdentity> IdentifyHandlers();

        IHandle<TEvent>? FindHandler(HandlerIdentity handlerIdentity);
    }
}
