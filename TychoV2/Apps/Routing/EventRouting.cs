using TychoV2.Events;
using TychoV2.Events.Registrating;
using TychoV2.Modules;

namespace TychoV2.Apps.Routing
{
    internal class EventRouting<TEvent> : IEventRouting
        where TEvent : class, IEvent
    {
        private readonly Registrator _registrator;

        public EventRouting(Registrator registrator)
        {
            _registrator = registrator;
        }

        public IEventRouting Forwards<Module>() 
            where Module : TychoModule
        {
            _registrator.ForwardEvent<TEvent, Module>();
            return this;
        }
    }
}
