using System.Collections.Generic;
using Tycho.Events;

namespace Tycho.Registry
{
    internal interface IEventHandlerRegistry
    {
        public void RegisterHandler<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>;

        public IReadOnlyCollection<EventHandlerIdentity> IdenitfyHandlers<TEvent>()
            where TEvent : class, IEvent;

        public IEventHandler GetHandler(EventHandlerIdentity eventHandlerId);
    }
}