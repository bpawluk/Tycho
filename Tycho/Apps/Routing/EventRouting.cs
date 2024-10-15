using Tycho.Events;
using Tycho.Events.Registrating;
using Tycho.Modules;

namespace Tycho.Apps.Routing
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