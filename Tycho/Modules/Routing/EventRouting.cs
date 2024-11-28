using System;
using Tycho.Events;
using Tycho.Events.Registrating;

namespace Tycho.Modules.Routing
{
    internal class EventRouting<TEvent> : IEventRouting<TEvent>
        where TEvent : class, IEvent
    {
        private readonly Registrator _registrator;

        public EventRouting(Registrator registrator)
        {
            _registrator = registrator;
        }

        public IEventRouting<TEvent> Exposes()
        {
            _registrator.ExposeEvent<TEvent>();
            return this;
        }

        public IEventRouting<TEvent> ExposesAs<TOtherEvent>(Func<TEvent, TOtherEvent> map)
            where TOtherEvent : class, IEvent
        {
            _registrator.ExposeEvent(map);
            return this;
        }

        public IEventRouting<TEvent> Forwards<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardEvent<TEvent, TModule>();
            return this;
        }

        public IEventRouting<TEvent> ForwardsAs<TOtherEvent, TModule>(Func<TEvent, TOtherEvent> map)
            where TOtherEvent : class, IEvent
            where TModule : TychoModule
        {
            _registrator.ForwardEvent<TEvent, TOtherEvent, TModule>(map);
            return this;
        }
    }
}