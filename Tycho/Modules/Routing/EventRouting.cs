using Tycho.Events;
using Tycho.Events.Registrating;

namespace Tycho.Modules.Routing
{
    internal class EventRouting<TEvent> : IEventRouting
        where TEvent : class, IEvent
    {
        private readonly Registrator _registrator;

        public EventRouting(Registrator registrator)
        {
            _registrator = registrator;
        }

        public IEventRouting Exposes()
        {
            _registrator.ExposeEvent<TEvent>();
            return this;
        }

        public IEventRouting Forwards<Module>() 
            where Module : TychoModule
        {
            _registrator.ForwardEvent<TEvent, Module>();
            return this;
        }
    }
}
