using System;
using System.Collections.Generic;

namespace TychoV2.Events.Router
{
    internal class EventRouter
    {
        public IReadOnlyCollection<string> IdentifyEventHandlers<TEvent>()
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
