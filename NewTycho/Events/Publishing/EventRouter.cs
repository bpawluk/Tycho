using System;

namespace NewTycho.Events.Publishing
{
    internal class EventRouter
    {
        public string[] IdentifyEventHandlers<TEvent>()
            where TEvent : class, IEvent
        {
            throw new NotImplementedException();
        }

        public IHandle<TEvent> GetEventHandler<TEvent>(string handlerId)
            where TEvent : class, IEvent
        {
            throw new NotImplementedException();
        }
    }
}
