using TychoV2.Events;
using TychoV2.Events.Registrating;

namespace TychoV2.Modules.Routing
{
    internal class EventRouting<TEvent> : IEventRouting
        where TEvent : class, IEvent
    {
        private readonly Registrator _registrator;

        public EventRouting(Registrator registrator)
        {
            _registrator = registrator;
        }

        public IEventRouting Expose()
        {
            _registrator.ExposeEvent<TEvent>();
            return this;
        }

        public IEventRouting Forward<Module>() 
            where Module : TychoModule
        {
            _registrator.ForwardEvent<TEvent, Module>();
            return this;
        }
    }
}
