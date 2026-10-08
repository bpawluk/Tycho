using Tycho.Events;

namespace Tycho.Identity.Events
{
    internal interface IEventHandlerProvider
    {
        public IEventHandler<TEvent> GetHandler<TEvent>(EventHandlerIdentity eventHandlerId) where TEvent : class, IEvent;
    }
}
