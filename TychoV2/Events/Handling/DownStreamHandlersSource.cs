using System.Collections.Generic;
using TychoV2.Events.Routing;
using TychoV2.Modules;

namespace TychoV2.Events.Handling
{
    internal class DownStreamHandlersSource<TEvent, TModule> : IHandlersSource<TEvent>
        where TEvent : class, IEvent
        where TModule : TychoModule
    {
        private readonly IEventRouter _submoduleEventRouter;

        public DownStreamHandlersSource(IModule<TModule> submodule)
        {
            _submoduleEventRouter = submodule.EventRouter;
        }

        public IReadOnlyCollection<HandlerIdentity> IdentifyHandlers() =>
            _submoduleEventRouter.IdentifyHandlers<TEvent>();

        public IHandle<TEvent>? FindHandler(HandlerIdentity handlerIdentity) =>
            _submoduleEventRouter.FindHandler<TEvent>(handlerIdentity);
    }
}
