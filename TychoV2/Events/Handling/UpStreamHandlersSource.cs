using System.Collections.Generic;
using TychoV2.Events.Routing;
using TychoV2.Structure;

namespace TychoV2.Events.Handling
{
    internal class UpStreamHandlersSource<TEvent> : IHandlersSource<TEvent>
        where TEvent : class, IEvent
    {
        private readonly IEventRouter _parentEventRouter;

        public UpStreamHandlersSource(IParent parent)
        {
            _parentEventRouter = parent.EventRouter;
        }

        public IReadOnlyCollection<HandlerIdentity> IdentifyHandlers() =>
            _parentEventRouter.IdentifyHandlers<TEvent>();

        public IHandle<TEvent>? FindHandler(HandlerIdentity handlerIdentity) => 
            _parentEventRouter.FindHandler<TEvent>(handlerIdentity);
    }
}
